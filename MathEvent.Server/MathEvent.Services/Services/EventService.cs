using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Events.DTOs;
using MathEvent.Converters.Events.Models;
using MathEvent.Converters.Files.Models;
using MathEvent.Converters.Others;
using MathEvent.Entities.Entities;
using MathEvent.Services.Results;
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
    /// TODO: уменьшить зависимость от других сервисов
    public class EventService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        private readonly IMapper _mapper;

        private readonly DataPathService _dataPathService;

        private const uint _breadcrumbRecursionDepth = 8;

        private const int _defaultEventSubsStatisticsTop = 10;

        public EventService(
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            DataPathService dataPathService)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _dataPathService = dataPathService;
        }

        /// <summary>
        /// Возвращает результат с набором событий, соответствующих фильтрам
        /// </summary>
        /// <param name="filters">Набор пар ключ-значение</param>
        /// <returns>Результат с набором событий, соответствующих фильтрам</returns>
        /// <remarks>Доступен только ключ "parent" с указанием значения - id родительского события</remarks>
        public async Task<IResult<IMessage, IEnumerable<EventReadModel>>> ListAsync(IDictionary<string, string> filters)
        {
            var events = await Filter(_repositoryWrapper.Event.FindAll(), filters)
                .OrderBy(e => e.StartDate)
                .ToListAsync();

            if (events is not null)
            {
                var eventsDTO = _mapper.Map<IEnumerable<EventDTO>>(events);

                return ResultFactory.GetSuccessfulResult(_mapper.Map<IEnumerable<EventReadModel>>(eventsDTO));
            }

            return ResultFactory.GetUnsuccessfulMessageResult<IEnumerable<EventReadModel>>(new List<IMessage>()
            {
                MessageFactory.GetSimpleMessage("402", "The list of events is empty")
            });
        }

        /// <summary>
        /// Возвращает результат с событием с указанным id
        /// </summary>
        /// <param name="id">id события, которое требуется получить</param>
        /// <returns>Результат с событием с указанным id</returns>
        public async Task<IResult<IMessage, EventWithUsersReadModel>> RetrieveAsync(int id)
        {
            var eventResult = await GetEventEntityAsync(id);

            if (!eventResult.Succeeded)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<EventWithUsersReadModel>(eventResult.Messages);
            }

            var eventEntity = eventResult.Entity;
            var eventDTO = _mapper.Map<EventWithUsersDTO>(eventEntity);
            var eventReadModel = _mapper.Map<EventWithUsersReadModel>(eventDTO);
            eventReadModel.OwnerId = (await GetEventOwnerAsync(eventReadModel.Id, Owner.Type.File)).Id;

            return ResultFactory.GetSuccessfulResult(eventReadModel);
        }

        /// <summary>
        /// Создает событие
        /// </summary>
        /// <param name="createModel">Модель нового события</param>
        /// <returns>Результат создания события</returns>
        public async Task<IResult<IMessage, EventWithUsersReadModel>> CreateAsync(EventCreateModel createModel)
        {
            var eventEntity = _mapper.Map<Event>(_mapper.Map<EventDTO>(createModel));

            if (eventEntity is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<EventWithUsersReadModel>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage(null, $"Errors when mapping model {createModel.Name}")
                });
            }

            var eventEntityDb = await _repositoryWrapper.Event.CreateAsync(eventEntity);

            if (eventEntityDb is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<EventWithUsersReadModel>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage(null, $"Errors when creating entity {eventEntity.Name}")
                });
            }

            await _repositoryWrapper.SaveAsync();

            if (await CreateEventOwnerAsync(eventEntityDb.Id, Owner.Type.File) is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<EventWithUsersReadModel>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage(null, $"Errors when creating an owner for event with id = {eventEntityDb.Id}")
                });
            }

            // TODO: где нить еще овнер нужен?
            EventWithUsersReadModel eventReadModel = _mapper.Map<EventWithUsersReadModel>(_mapper.Map<EventWithUsersDTO>(eventEntityDb));
            eventReadModel.OwnerId = (await GetEventOwnerAsync(eventReadModel.Id, Owner.Type.File)).Id;

            return ResultFactory.GetSuccessfulResult(eventReadModel);
        }

        /// <summary>
        /// Обновляет событие
        /// </summary>
        /// <param name="id">id события, которое требуется обновить</param>
        /// <param name="updateModel">Модель для обновления события</param>
        /// <returns>Результат обновления события</returns>
        public async Task<IResult<IMessage, EventWithUsersReadModel>> UpdateAsync(int id, EventUpdateModel updateModel)
        {
            var eventResult = await GetEventEntityAsync(id);

            if (!eventResult.Succeeded)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<EventWithUsersReadModel>(eventResult.Messages);
            }

            var eventEntity = eventResult.Entity;

            if (updateModel.Hierarchy is null && eventEntity.Hierarchy is not null)
            {
                var children = await _repositoryWrapper.Event
                    .FindByCondition(ev => ev.ParentId == eventEntity.Id)
                    .ToListAsync();

                if (children.Count > 0)
                {
                    return ResultFactory.GetUnsuccessfulMessageResult<EventWithUsersReadModel>(new List<IMessage>()
                    {
                        MessageFactory.GetSimpleMessage("400", $"Event with the ID {id} has child elements")
                    });
                }
            }

            await CreateNewSubscriptions(updateModel.ApplicationUsers, id);
            await CreateNewManagers(updateModel.Managers, id);
            var eventDTO = _mapper.Map<EventWithUsersDTO>(eventEntity);
            _mapper.Map(updateModel, eventDTO);
            _mapper.Map(eventDTO, eventEntity);
            _repositoryWrapper.Event.Update(eventEntity);
            await _repositoryWrapper.SaveAsync();

            EventWithUsersReadModel eventReadModel = _mapper.Map<EventWithUsersReadModel>(eventDTO);
            eventReadModel.OwnerId = (await GetEventOwnerAsync(eventReadModel.Id, Owner.Type.File)).Id;

            return ResultFactory.GetSuccessfulResult(eventReadModel);
        }

        /// <summary>
        /// Удаляет событие с указанным id
        /// </summary>
        /// <param name="id">id события, которое требуется удалить</param>
        /// <returns>Результат удаления события</returns>
        /// <remarks>При удалении события удаляется аватар события</remarks>
        /// TODO: удаляются ли файлы события?
        public async Task<IResult<IMessage, EventWithUsersReadModel>> DeleteAsync(int id)
        {
            var eventResult = await GetEventEntityAsync(id);

            if (!eventResult.Succeeded)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<EventWithUsersReadModel>(eventResult.Messages);
            }

            var eventEntity = eventResult.Entity;
            var avatarPath = eventEntity.AvatarPath;
            _repositoryWrapper.Event.Delete(eventEntity);
            await _repositoryWrapper.SaveAsync();

            if (avatarPath is not null)
            {
                _dataPathService.DeleteFile(avatarPath, out string deleteMessage);

                if (deleteMessage is not null)
                {
                    return ResultFactory.GetUnsuccessfulMessageResult<EventWithUsersReadModel>(new List<IMessage>()
                    {
                        MessageFactory.GetSimpleMessage("400", deleteMessage)
                    });
                }
            }

            return ResultFactory.GetSuccessfulResult((EventWithUsersReadModel)null);
        }

        /// <summary>
        /// Загружает аватар для события
        /// </summary>
        /// <param name="id">id события</param>
        /// <param name="file">Файл - аватар события</param>
        /// <param name="fileCreateModel">Модель создания события</param>
        /// <returns>Результат загрузки аватара для события</returns>
        /// <remarks>Предыдущий аватар удаляется. Модель создания события требуется для передачи данных об авторе</remarks>
        public async Task<IResult<IMessage, EventWithUsersReadModel>> UploadAvatar(int id, IFormFile file, FileCreateModel fileCreateModel)
        {
            var eventResult = await GetEventEntityAsync(id);

            if (!eventResult.Succeeded)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<EventWithUsersReadModel>(eventResult.Messages);
            }

            var eventEntity = eventResult.Entity;
            var fileResult = await _dataPathService.Create(file, fileCreateModel.AuthorId);

            if (!fileResult.Succeeded)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<EventWithUsersReadModel>(fileResult.Messages);
            }

            if (eventEntity.AvatarPath is not null)
            {
                _dataPathService.DeleteFile(eventEntity.AvatarPath, out string deleteMessage);

                if (deleteMessage is not null)
                {
                    return ResultFactory.GetUnsuccessfulMessageResult<EventWithUsersReadModel>(new List<IMessage>
                    {
                        MessageFactory.GetSimpleMessage("400", deleteMessage)
                    });
                }

                eventEntity.AvatarPath = null;
            }

            eventEntity.AvatarPath = fileResult.Entity;
            _repositoryWrapper.Event.Update(eventEntity);

            await _repositoryWrapper.SaveAsync();

            var eventDTO = _mapper.Map<EventWithUsersDTO>(eventEntity);
            var eventReadModel = _mapper.Map<EventWithUsersReadModel>(eventDTO);
            eventReadModel.OwnerId = (await GetEventOwnerAsync(eventReadModel.Id, Owner.Type.File)).Id;

            return ResultFactory.GetSuccessfulResult(eventReadModel);
        }

        /// <summary>
        /// Возвращает результат с событием с указанным id
        /// </summary>
        /// <param name="id">id события, которое требуется получить</param>
        /// <returns>Результат с событием с указанным id</returns>
        public async Task<IResult<IMessage, Event>> GetEventEntityAsync(int id)
        {
            var eventEntity = await _repositoryWrapper.Event
                .FindByCondition(ev => ev.Id == id)
                .SingleOrDefaultAsync();

            if (eventEntity is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<Event>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("404", "Event not found")
                });
            }

            return ResultFactory.GetSuccessfulResult(eventEntity);
        }

        /// <summary>
        /// Вовзращает набор-цепочку родительских событий в виде хлебных крошек до события с указанным id
        /// </summary>
        /// <param name="id">id события, для которого требуется найти хлебные крошки</param>
        /// <returns>Набор-цепочка родительских событий в виде хлебных крошек</returns>
        public async Task<IResult<IMessage, IEnumerable<Breadcrumb>>> GetBreadcrumbs(int id)
        {
            var breadcrumbs = new Stack<Breadcrumb>();
            var eventResult = await GetEventEntityAsync(id);

            if (!eventResult.Succeeded)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<IEnumerable<Breadcrumb>>(eventResult.Messages);
            }

            var currentEvent = eventResult.Entity;

            breadcrumbs.Push(new Breadcrumb
            {
                Id = currentEvent.Id,
                Name = currentEvent.Name
            });

            var parent = await _repositoryWrapper.Event
                .FindByCondition(e => e.Id == currentEvent.ParentId)
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

            return ResultFactory.GetSuccessfulResult(breadcrumbs.AsEnumerable());
        }

        /// <summary>
        /// Возращает результат со статистикой по событию
        /// </summary>
        /// <param name="id">id события, статистику по которому требуется получить</param>
        /// <returns>Результат со статистикой по событию</returns>
        public async Task<IResult<IMessage, IEnumerable<SimpleStatistics>>> GetEventStatistics(int id)
        {
            var simpleStatistics = await GetEventSubscribersByOrganizationStatistics(id);

            if (simpleStatistics is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<IEnumerable<SimpleStatistics>>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("400", $"Errors when getting a statistics for event with id = {id}")
                });
            }

            IEnumerable<SimpleStatistics> eventStatistics = new List<SimpleStatistics>()
            {
                simpleStatistics
            };

            return ResultFactory.GetSuccessfulResult(eventStatistics);
        }

        /// <summary>
        /// Возвращает результат с набором статистик по событиям
        /// </summary>
        /// <param name="filters">Набор параметров в виде пар ключ-значение</param>
        /// <returns>Результат с набором статистик по событиям</returns>
        /// <remarks>Параметр "eventSubsStatisticsTop" задает топ событий, который требуется получить</remarks>
        public async Task<IResult<IMessage, IEnumerable<SimpleStatistics>>> GetSimpleStatistics(IDictionary<string, string> filters)
        {
            int eventSubsStatisticsTop = _defaultEventSubsStatisticsTop;

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

            var simpleStatistics = new List<SimpleStatistics>();
            var subsStatistics = await GetSubcribersStatistics(eventSubsStatisticsTop);

            if (subsStatistics is not null)
            {
                simpleStatistics.Add(subsStatistics);
            }

            var monthStatistics = await GetMonthStatistics();

            if (monthStatistics is not null)
            {
                simpleStatistics.Add(monthStatistics);
            }

            if (simpleStatistics.Count < 1)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<IEnumerable<SimpleStatistics>>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("400", "Errors when getting a statistics for events")
                });
            }

            return ResultFactory.GetSuccessfulResult(simpleStatistics.AsEnumerable());
        }

        private async Task<SimpleStatistics> GetEventSubscribersByOrganizationStatistics(int eventId)
        {
            var eventResult = await GetEventEntityAsync(eventId);

            if (!eventResult.Succeeded)
            {
                return null;
            }

            var eventEntity = eventResult.Entity;

            var statistics = new SimpleStatistics
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
                    // мб вообще статистику в отдельный сервис вынести потом, а не просто избавиться здесь от UserService
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

        private async Task<SimpleStatistics> GetSubcribersStatistics(int number)
        {
            var statistics = new SimpleStatistics
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

                var eventResult = await GetEventEntityAsync(entry.Key);

                if (!eventResult.Succeeded)
                {
                    return null;
                }

                var eventEntity = eventResult.Entity;

                statistics.Data.Add(
                    new ChartDataPiece
                    {
                        X = eventEntity.Name,
                        Y = entry.Value
                    });
            }

            return statistics;
        }

        private async Task<SimpleStatistics> GetMonthStatistics()
        {
            var statistics = new SimpleStatistics
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
            }

            return eventQuery;
        }

        /// <summary>
        /// Создает владельца-событие
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        /// <param name="type">Тип обладаемой сущности</param>
        /// <returns>Владелец</returns>
        private async Task<Owner> CreateEventOwnerAsync(int id, Owner.Type type)
        {
            var owner = await _repositoryWrapper.Owner.CreateAsync(
                new Owner
                {
                    EventId = id,
                    OwnedType = type
                });
            await _repositoryWrapper.SaveAsync();

            return owner;
        }

        private async Task<Owner> GetEventOwnerAsync(int id, Owner.Type type)
        {
            var owner = _repositoryWrapper.Owner
                    .FindByCondition(ow => ow.EventId == id && ow.OwnedType == type)
                    .SingleOrDefault();

            if (owner is null)
            {
                owner = await CreateEventOwnerAsync(id, type);
            }

            return owner;
        }
    }
}
