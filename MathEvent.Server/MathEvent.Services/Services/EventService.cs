using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Contracts.Services;
using MathEvent.DTOs.Events;
using MathEvent.Entities.Entities;
using MathEvent.Models.Events;
using MathEvent.Models.Files;
using MathEvent.Models.Others;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MathEvent.Services.Services
{
    /// <summary>
    /// Сервис по выполнению действий над событиями
    /// </summary>
    public class EventService : IEventService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        private readonly IMapper _mapper;

        private readonly IDataPathWorker _dataPathService;

        private const uint _breadcrumbRecursionDepth = 8;

        private const int _defaultEventSubsStatisticsTop = 10;

        public EventService(
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            IDataPathWorker dataPathService)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _dataPathService = dataPathService;
        }

        /// <summary>
        /// Возвращает набор событий, соответствующих фильтрам
        /// </summary>
        /// <param name="filters">Набор пар ключ-значение</param>
        /// <returns>Набор событий, соответствующих фильтрам</returns>
        public async Task<IEnumerable<EventReadModel>> ListAsync(IDictionary<string, string> filters)
        {
            var events = await Filter(_repositoryWrapper.Event.FindAll(), filters)
                .OrderBy(e => e.StartDate)
                .ToListAsync();

            var eventDTOs = _mapper.Map<IEnumerable<EventDTO>>(events);
            var eventReadModels = _mapper.Map<IEnumerable<EventReadModel>>(eventDTOs);

            return eventReadModels;
        }

        /// <summary>
        /// Возвращает событие по указанному id
        /// </summary>
        /// <param name="id">id события, которое требуется получить</param>
        /// <returns>Событие с указанным id</returns>
        public async Task<EventWithUsersReadModel> RetrieveAsync(int id)
        {
            var eventEntity = await GetEventEntityAsync(id);

            if (eventEntity is null)
            {
                return null;
            }

            var eventDTO = _mapper.Map<EventWithUsersDTO>(eventEntity);
            var eventReadModel = _mapper.Map<EventWithUsersReadModel>(eventDTO);

            return eventReadModel;
        }

        /// <summary>
        /// Создает событие
        /// </summary>
        /// <param name="createModel">Модель нового события</param>
        /// <returns>Cозданное событие</returns>
        public async Task<EventWithUsersReadModel> CreateAsync(EventCreateModel createModel)
        {
            if (string.IsNullOrEmpty(createModel.AuthorId))
            {
                throw new Exception("AuthorId is empty");
            }

            var eventEntity = _mapper.Map<Event>(_mapper.Map<EventDTO>(createModel));
            var eventEntityDb = await _repositoryWrapper.Event.CreateAsync(eventEntity);

            if (eventEntityDb is null)
            {
                throw new Exception("Errors while creating event");
            }

            await _repositoryWrapper.SaveAsync();

            await CreateNewManagers(new string[] { createModel.AuthorId }, eventEntityDb.Id);

            await _repositoryWrapper.SaveAsync();

            var eventReadModel = _mapper.Map<EventWithUsersReadModel>(_mapper.Map<EventWithUsersDTO>(eventEntityDb));

            return eventReadModel;
        }

        /// <summary>
        /// Обновляет событие
        /// </summary>
        /// <param name="id">id события, которое требуется обновить</param>
        /// <param name="updateModel">Модель для обновления события</param>
        /// <returns>Обновленное событие</returns>
        public async Task<EventWithUsersReadModel> UpdateAsync(int id, EventUpdateModel updateModel)
        {
            if (updateModel.Managers.Count < 1)
            {
                throw new Exception("The number of managers is less than 1");
            }

            var eventEntity = await GetEventEntityAsync(id);

            if (eventEntity is null)
            {
                throw new Exception($"Event with id={id} is not exists");
            }

            if (updateModel.Hierarchy is null && eventEntity.Hierarchy is not null)
            {
                var children = await _repositoryWrapper.Event
                    .FindByCondition(ev => ev.ParentId == eventEntity.Id)
                    .ToListAsync();

                if (children.Count > 0)
                {
                    throw new Exception($"Event with the id={id} has child elements");
                }
            }

            await CreateNewSubscriptions(updateModel.ApplicationUsers, id);

            await CreateNewManagers(updateModel.Managers, id);

            var eventDTO = _mapper.Map<EventWithUsersDTO>(eventEntity);
            _mapper.Map(updateModel, eventDTO);
            _mapper.Map(eventDTO, eventEntity);

            _repositoryWrapper.Event.Update(eventEntity);
            await _repositoryWrapper.SaveAsync();

            var eventReadModel = _mapper.Map<EventWithUsersReadModel>(eventDTO);

            return eventReadModel;
        }

        /// <summary>
        /// Удаляет событие с указанным id
        /// </summary>
        /// <param name="id">id события, которое требуется удалить</param>
        /// <returns></returns>
        /// <remarks>При удалении события удаляется аватар события</remarks>
        /// TODO: удаляются ли файлы события?
        public async Task DeleteAsync(int id)
        {
            var eventEntity = await GetEventEntityAsync(id);

            if (eventEntity is null)
            {
                throw new Exception($"Event with id={id} is not exists");
            }

            var avatarPath = eventEntity.AvatarPath;

            _repositoryWrapper.Event.Delete(eventEntity);
            await _repositoryWrapper.SaveAsync();

            if (avatarPath is not null)
            {
                _dataPathService.DeleteFile(avatarPath, out string deleteMessage);

                if (deleteMessage is not null)
                {
                    throw new Exception(deleteMessage);
                }
            }
        }

        /// <summary>
        /// Загружает аватар для события
        /// </summary>
        /// <param name="id">id события</param>
        /// <param name="file">Файл - аватар события</param>
        /// <param name="fileCreateModel">Модель создания события</param>
        /// <returns>Модель события с обновленным аватаром</returns>
        /// <remarks>Предыдущий аватар удаляется. Модель создания события требуется для передачи данных об авторе</remarks>
        public async Task<EventWithUsersReadModel> UploadAvatar(int id, IFormFile file, FileCreateModel fileCreateModel)
        {
            var eventEntity = await GetEventEntityAsync(id);

            if (eventEntity is null)
            {
                throw new Exception($"Event with id={id} is not exists");
            }

            var filePath = await _dataPathService.Create(file, fileCreateModel.AuthorId);

            if (filePath is null)
            {
                throw new Exception("Errors while creating a file");
            }

            if (eventEntity.AvatarPath is not null)
            {
                _dataPathService.DeleteFile(eventEntity.AvatarPath, out string deleteMessage);

                if (deleteMessage is not null)
                {
                    throw new Exception(deleteMessage);
                }

                eventEntity.AvatarPath = null;
            }

            eventEntity.AvatarPath = filePath;

            _repositoryWrapper.Event.Update(eventEntity);
            await _repositoryWrapper.SaveAsync();

            var eventDTO = _mapper.Map<EventWithUsersDTO>(eventEntity);
            var eventReadModel = _mapper.Map<EventWithUsersReadModel>(eventDTO);

            return eventReadModel;
        }

        /// <summary>
        /// Ищет дочерние события
        /// </summary>
        /// <param name="id">id события</param>
        /// <returns>Дочерние события</returns>
        public async Task<IEnumerable<EventReadModel>> GetChildEvents(int id)
        {
            var childEvents = await _repositoryWrapper.Event
                .FindByCondition(ev => ev.ParentId == id)
                .ToListAsync();

            var eventsDTO = _mapper.Map<IEnumerable<EventDTO>>(childEvents);
            var eventsReadModels = _mapper.Map<IEnumerable<EventReadModel>>(eventsDTO);

            return eventsReadModels;
        }

        /// <summary>
        /// Возвращает события в указанном промежутке времени
        /// </summary>
        /// <param name="start">Начало интервала времени</param>
        /// <param name="end">Конец интервала времени</param>
        /// <returns>События в интервале времени (по дате)</returns>
        public async Task<IEnumerable<EventWithUsersReadModel>> GetEventsByDate(DateTime start, DateTime end)
        {
            var events = await _repositoryWrapper.Event
                .FindByCondition(ev => ev.StartDate.Date >= start.Date && ev.StartDate.Date <= end.Date)
                .ToListAsync();

            var eventsDTO = _mapper.Map<IEnumerable<EventWithUsersDTO>>(events);
            var eventsReadModels = _mapper.Map<IEnumerable<EventWithUsersReadModel>>(eventsDTO);

            return eventsReadModels;
        }

        /// <summary>
        /// Вовзращает набор-цепочку родительских событий в виде хлебных крошек до события с указанным id
        /// </summary>
        /// <param name="id">id события, для которого требуется найти хлебные крошки</param>
        /// <returns>Набор-цепочка родительских событий в виде хлебных крошек</returns>
        public async Task<IEnumerable<Breadcrumb>> GetBreadcrumbs(int id)
        {
            var breadcrumbs = new Stack<Breadcrumb>();
            var eventEntity = await GetEventEntityAsync(id);

            if (eventEntity is null)
            {
                throw new Exception($"Event with id={id} is not exists");
            }

            breadcrumbs.Push(new Breadcrumb
            {
                Id = eventEntity.Id,
                Name = eventEntity.Name
            });

            var parent = await _repositoryWrapper.Event
                .FindByCondition(e => e.Id == eventEntity.ParentId)
                .SingleOrDefaultAsync();
            var depth = _breadcrumbRecursionDepth;

            while (parent != null && depth > 0)
            {
                breadcrumbs.Push(new Breadcrumb
                {
                    Id = parent.Id,
                    Name = parent.Name
                });
                parent = await _repositoryWrapper.Event
                    .FindByCondition(e => e.Id == parent.ParentId)
                    .SingleOrDefaultAsync();
                depth--;
            }

            return breadcrumbs;
        }

        /// <summary>
        /// Возвращает статистику по событию
        /// </summary>
        /// <param name="id">id события, статистику по которому требуется получить</param>
        /// <returns>Статистика по событию</returns>
        public async Task<IEnumerable<ChartData>> GetEventStatistics(int id)
        {
            var eventSubscribersByOrganizationStats = await GetEventSubscribersByOrganizationStatistics(id);

            var eventStatistics = new List<ChartData>()
            {
                eventSubscribersByOrganizationStats
            };

            return eventStatistics;
        }

        /// <summary>
        /// Возвращает набор статистик по событиям
        /// </summary>
        /// <param name="filters">Набор параметров в виде пар ключ-значение</param>
        /// <returns>Набор статистик по событиям</returns>
        /// <remarks>Параметр "eventSubsStatisticsTop" задает топ событий, который требуется получить</remarks>
        public async Task<IEnumerable<ChartData>> GetEventsStatistics(IDictionary<string, string> filters)
        {
            var eventSubsStatisticsTop = _defaultEventSubsStatisticsTop;

            if (filters is not null)
            {
                if (filters.TryGetValue("eventSubsStatisticsTop", out string eventSubsStatisticsTopParam))
                {
                    if (int.TryParse(eventSubsStatisticsTopParam, out int eventSubsStatisticsTopValue))
                    {
                        eventSubsStatisticsTop = eventSubsStatisticsTopValue;
                    }
                }
            }

            var evntsStatistics = new List<ChartData>();
            var subsStatistics = await GetSubcribersStatistics(eventSubsStatisticsTop);

            if (subsStatistics is not null)
            {
                evntsStatistics.Add(subsStatistics);
            }

            var monthStatistics = await GetMonthStatistics();

            if (monthStatistics is not null)
            {
                evntsStatistics.Add(monthStatistics);
            }

            return evntsStatistics;
        }

        /// <summary>
        /// Возвращает количество событий по датам
        /// </summary>
        /// <param name="dates">Словарь с граничными датами периода поиска</param>
        /// <returns>Словарь с количеством событий по датам</returns>
        public async Task<IDictionary<DateTime, int>> GetEventsCountByDateAsync(IDictionary<string, string> dates)
        {
            var startDateFrom = DateTime.Now.AddDays(-100).Date;
            var startDateTo = DateTime.Now.AddDays(100).Date;

            if (dates.TryGetValue("startDateFrom", out string startDateFromParam))
            {
                startDateFrom = DateTime.Parse(startDateFromParam);
            }

            if (dates.TryGetValue("startDateTo", out string startDateToParam))
            {
                startDateTo = DateTime.Parse(startDateToParam);
            }

            return await _repositoryWrapper.Event
                .FindByCondition(ev => ev.StartDate.Date >= startDateFrom.Date && ev.StartDate.Date <= startDateTo.Date)
                .GroupBy(ev => ev.StartDate.Date)
                .Select(g => new { date = g.Key, count = g.Count() })
                .ToDictionaryAsync(k => k.date, i => i.count);
        }

        /// <summary>
        /// Возвращает событие с указанным id
        /// </summary>
        /// <param name="id">id события, которое требуется получить</param>
        /// <returns>Событие с указанным id</returns>
        private async Task<Event> GetEventEntityAsync(int id)
        {
            var eventEntity = await _repositoryWrapper.Event
                .FindByCondition(ev => ev.Id == id)
                .SingleOrDefaultAsync();

            return eventEntity;
        }

        private async Task<ChartData> GetEventSubscribersByOrganizationStatistics(int eventId)
        {
            var eventEntity = await GetEventEntityAsync(eventId);

            if (eventEntity is null)
            {
                throw new Exception($"Event with id={eventId} is not exists");
            }

            var statistics = new ChartData
            {
                Title = eventEntity.Hierarchy is null
                ? $"Подписчики события по организациям"
                : $"Подписчики вложенных \"листовых\" событий по организациям",
                Data = new List<ChartDataPiece>()
            };

            var organizationSubcribersCount = await GetEventSubscribersByOrganizationRoot(eventEntity);

            foreach (var entry in organizationSubcribersCount)
            {
                var name = "Без организации";

                if (entry.Key != -1)
                {
                    var organization = await _repositoryWrapper.Organization
                        .FindByCondition(org => org.Id == entry.Key)
                        .SingleOrDefaultAsync();
                    name = organization.Name;
                }

                statistics.Data.Add(
                    new ChartDataPiece
                    {
                        X = name,
                        Y = entry.Value
                    });
            }

            return statistics;
        }

        private async Task<IDictionary<int, int>> GetEventSubscribersByOrganizationRoot(Event ev)
        {
            var organizationSubcribersCount = new Dictionary<int, int>();

            return await GetEventSubscribersByOrganization(ev, 0, organizationSubcribersCount);
        }

        private async Task<IDictionary<int, int>> GetEventSubscribersByOrganization(Event ev, int level, IDictionary<int, int> orgSubsCount)
        {
            // TODO: мб вместо обращения к репозиторию, задействовать другие сервисы
            if (level > _breadcrumbRecursionDepth)
            {
                return orgSubsCount;
            }

            if (ev.Hierarchy is null)
            {
                var subscriptions = await _repositoryWrapper.Subscription
                    .FindByCondition(s => s.EventId == ev.Id)
                    .ToListAsync();

                var subscribers = new List<ApplicationUser>();

                foreach (var subscription in subscriptions)
                {
                    subscribers.Add(await _repositoryWrapper.User
                        .FindByCondition(user => user.Id == subscription.ApplicationUserId)
                        .SingleOrDefaultAsync());
                }

                foreach (var user in subscribers)
                {
                    int orgId = -1;

                    if (user.OrganizationId != null)
                    {
                        orgId = user.OrganizationId.Value;
                    }

                    if (orgSubsCount.ContainsKey(orgId))
                    {
                        orgSubsCount[orgId]++;
                    }
                    else
                    {
                        orgSubsCount.Add(orgId, 1);
                    }
                }
            }

            var children = await _repositoryWrapper.Event
                .FindByCondition(e => e.ParentId == ev.Id)
                .ToListAsync();

            foreach (var child in children)
            {
                await GetEventSubscribersByOrganization(child, level + 1, orgSubsCount);
            }

            return orgSubsCount;
        }

        private async Task<ChartData> GetSubcribersStatistics(int number)
        {
            var statistics = new ChartData
            {
                Title = $"Популярность событий по подписчикам",
                Data = new List<ChartDataPiece>()
            };

            var subscribersCountPerEvent = await _repositoryWrapper.Subscription
                .FindAll()
                .GroupBy(s => s.EventId)
                .Select(g => new { eventId = g.Key, count = g.Count() })
                .OrderBy(g => g.count)
                .Take(number)
                .ToDictionaryAsync(k => k.eventId, i => i.count);

            foreach (var entry in subscribersCountPerEvent)
            {

                var eventEntity = await GetEventEntityAsync(entry.Key);

                if (eventEntity is null)
                {
                    throw new Exception($"Event with id={entry.Key} is not exists");
                }

                statistics.Data.Add(
                    new ChartDataPiece
                    {
                        X = eventEntity.Name,
                        Y = entry.Value
                    });
            }

            return statistics;
        }

        private async Task<ChartData> GetMonthStatistics()
        {
            var statistics = new ChartData
            {
                Title = $"Самые загруженные месяцы по количеству событий за последний год",
                Type = ChartTypes.Bar,
                Data = new List<ChartDataPiece>()
            };

            var numberOfEventsPerMonthResult = await _repositoryWrapper.Event
                .FindByCondition(e => e.StartDate >= DateTime.Now.AddYears(-1))
                .GroupBy(e => e.StartDate.Month)
                .Select(g => new { month = g.Key, count = g.Count() })
                .ToDictionaryAsync(k => k.month, i => i.count);

            foreach (var entry in numberOfEventsPerMonthResult)
            {
                statistics.Data.Add(
                    new ChartDataPiece
                    {
                        X = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(entry.Key),
                        Y = entry.Value
                    });
            }

            return statistics;
        }

        private async Task CreateNewSubscriptions(IEnumerable<string> newIds, int eventId)
        {
            await _repositoryWrapper.Subscription
                .FindByCondition(s => s.EventId == eventId)
                .ForEachAsync(s => _repositoryWrapper.Subscription.Delete(s));

            foreach (var userId in newIds)
            {
                await _repositoryWrapper.Subscription
                    .CreateAsync(new Subscription()
                    {
                        ApplicationUserId = userId,
                        EventId = eventId
                    });
            }
        }

        private async Task CreateNewManagers(IEnumerable<string> newIds, int eventId)
        {
            await _repositoryWrapper.Management
                .FindByCondition(m => m.EventId == eventId)
                .ForEachAsync(m => _repositoryWrapper.Management.Delete(m));

            foreach (var userId in newIds)
            {
                await _repositoryWrapper.Management
                    .CreateAsync(new Management()
                    {
                        ApplicationUserId = userId,
                        EventId = eventId
                    });
            }
        }

        private static IQueryable<Event> Filter(IQueryable<Event> eventQuery, IDictionary<string, string> filters)
        {
            if (filters is not null)
            {
                if (filters.TryGetValue("parent", out string parentParam))
                {
                    if (int.TryParse(parentParam, out int parentId))
                    {
                        eventQuery = eventQuery.Where(f => f.ParentId == parentId);
                    }
                    else
                    {
                        eventQuery = eventQuery.Where(f => f.ParentId == null);
                    }
                }

                if (filters.TryGetValue("organization", out string organizationParam))
                {
                    if (int.TryParse(organizationParam, out int organizationId))
                    {
                        eventQuery = eventQuery.Where(f => f.OrganizationId == organizationId);
                    }
                }

                if (filters.TryGetValue("startDateFrom", out string startDateFromParam))
                {
                    if (DateTime.TryParse(startDateFromParam, out DateTime startDateFrom))
                    {
                        eventQuery = eventQuery.Where(f => f.StartDate >= startDateFrom);
                    }
                }

                if (filters.TryGetValue("startDateTo", out string startDateToParam))
                {
                    if (DateTime.TryParse(startDateToParam, out DateTime startDateTo))
                    {
                        eventQuery = eventQuery.Where(f => f.StartDate <= startDateTo);
                    }
                }
            }

            return eventQuery;
        }
    }
}
