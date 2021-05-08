using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Events.DTOs;
using MathEvent.Converters.Events.Models;
using MathEvent.Converters.Others;
using MathEvent.Entities.Entities;
using MathEvent.Services.Messages;
using MathEvent.Services.Results;
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

        private readonly IOwnerService _ownerService;

        private readonly IUserService _userService;

        private readonly IOrganizationService _organizationService;

        public EventService(IRepositoryWrapper repositoryWrapper, IMapper mapper, IOwnerService ownerService, IUserService userService, IOrganizationService organizationService)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _ownerService = ownerService;
            _userService = userService;
            _organizationService = organizationService;
        }

        public async Task<IEnumerable<EventReadModel>> ListAsync(IDictionary<string, string> filters)
        {
            var events = await Filter(_repositoryWrapper.Event.FindAll(), filters)
                .OrderBy(e => e.StartDate)
                .ToListAsync();

            if (events is not null)
            {
                var eventsDTO = _mapper.Map<IEnumerable<EventDTO>>(events);

                return _mapper.Map<IEnumerable<EventReadModel>>(eventsDTO);
            }

            return null;
        }

        public async Task<EventWithUsersReadModel> RetrieveAsync(int id)
        {
            var eventEntity = await GetEventEntityAsync(id);

            if (eventEntity is not null)
            {
                var eventDTO = _mapper.Map<EventWithUsersDTO>(eventEntity);
                var eventReadModel = _mapper.Map<EventWithUsersReadModel>(eventDTO);
                eventReadModel.OwnerId = (await _ownerService.GetEventOwnerAsync(eventReadModel.Id, Owner.Type.File)).Id;

                return eventReadModel;
            }

            return null;
        }

        public async Task<AResult<IMessage, EventWithUsersReadModel>> CreateAsync(EventCreateModel createModel)
        {
            var eventEntity = _mapper.Map<Event>(_mapper.Map<EventDTO>(createModel));

            if (eventEntity is null)
            {
                return new MessageResult<EventWithUsersReadModel>
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>()
                    {
                        new SimpleMessage
                        {
                            Message = $"Errors when mapping model {createModel.Name}"
                        }
                    }
                };

            }

            var eventEntityDb = await _repositoryWrapper.Event.CreateAsync(eventEntity);

            if (eventEntityDb is null)
            {
                return new MessageResult<EventWithUsersReadModel>
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>()
                    {
                        new SimpleMessage
                        {
                            Message = $"Errors when creating entity {eventEntity.Name}"
                        }
                    }
                };
            }

            await _repositoryWrapper.SaveAsync();

            if (await _ownerService.CreateEventOwner(eventEntityDb.Id, Owner.Type.File) is null)
            {
                return new MessageResult<EventWithUsersReadModel>
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>()
                    {
                        new SimpleMessage
                        {
                            Message = $"Errors when creating an owner for event with id = {eventEntityDb.Id}"
                        }
                    }
                };
            }

            // TODO: где нить еще овнер нужен?
            EventWithUsersReadModel eventReadModel = _mapper.Map<EventWithUsersReadModel>(_mapper.Map<EventWithUsersDTO>(eventEntityDb));
            eventReadModel.OwnerId = (await _ownerService.GetEventOwnerAsync(eventReadModel.Id, Owner.Type.File)).Id;

            return new MessageResult<EventWithUsersReadModel>
            {
                Succeeded = true,
                Entity = eventReadModel
            };
        }

        public async Task<AResult<IMessage, EventWithUsersReadModel>> UpdateAsync(int id, EventUpdateModel updateModel)
        {
            var eventEntity = await GetEventEntityAsync(id);

            if (eventEntity is null)
            {
                return new MessageResult<EventWithUsersReadModel>
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>
                    {
                        new SimpleMessage
                        {
                            Code = "404",
                            Message = $"Event with the ID {id} not found"
                        }
                    }
                };
            }

            await CreateNewSubscriptions(updateModel.ApplicationUsers, id);
            await CreateNewManagers(updateModel.Managers, id);
            var eventDTO = _mapper.Map<EventWithUsersDTO>(eventEntity);
            _mapper.Map(updateModel, eventDTO);
            _mapper.Map(eventDTO, eventEntity);
            _repositoryWrapper.Event.Update(eventEntity);
            await _repositoryWrapper.SaveAsync();

            EventWithUsersReadModel eventReadModel = _mapper.Map<EventWithUsersReadModel>(eventDTO);
            eventReadModel.OwnerId = (await _ownerService.GetEventOwnerAsync(eventReadModel.Id, Owner.Type.File)).Id;

            return new MessageResult<EventWithUsersReadModel>
            {
                Succeeded = true,
                Entity = eventReadModel
            };
        }

        public async Task<AResult<IMessage, EventWithUsersReadModel>> DeleteAsync(int id)
        {
            var eventEntity = await GetEventEntityAsync(id);

            if (eventEntity is null)
            {
                return new MessageResult<EventWithUsersReadModel>
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>
                    {
                        new SimpleMessage
                        {
                            Code = "404",
                            Message = $"Event with the ID {id} not found"
                        }
                    }
                };
            }

            _repositoryWrapper.Event.Delete(eventEntity);
            await _repositoryWrapper.SaveAsync();

            return new MessageResult<EventWithUsersReadModel> { Succeeded = true };
        }

        public async Task<Event> GetEventEntityAsync(int id)
        {
            return await _repositoryWrapper.Event
                .FindByCondition(ev => ev.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<AResult<IMessage, IEnumerable<Breadcrumb>>> GetBreadcrumbs(int id)
        {
            var breadcrumbs = new Stack<Breadcrumb>();
            var currentEvent = await GetEventEntityAsync(id);

            if (currentEvent == null)
            {
                return new MessageResult<IEnumerable<Breadcrumb>>
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>
                    {
                        new SimpleMessage
                        {
                            Code = "404",
                            Message = $"Event with the ID {id} not found"
                        }
                    }
                };
            }

            breadcrumbs.Push(new Breadcrumb
            {
                Id = currentEvent.Id,
                Name = currentEvent.Name
            });

            var parent = await _repositoryWrapper.Event
                .FindByCondition(e => e.Id == currentEvent.ParentId)
                .SingleOrDefaultAsync();
            var depth = 8;

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

            return new MessageResult<IEnumerable<Breadcrumb>>
            {
                Succeeded = true,
                Entity = breadcrumbs.ToList()
            };
        }

        public async Task<IEnumerable<SimpleStatistics>> GetEventStatistics(int id)
        {
            ICollection<SimpleStatistics> eventStatistics = new List<SimpleStatistics>
            {
                await GetEventSubscribersByOrganizationStatistics(id)
            };

            return eventStatistics;
        }

        public async Task<IEnumerable<SimpleStatistics>> GetSimpleStatistics(IDictionary<string, string> filters)
        {
            int eventSubsStatisticsTop = 10;

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

            ICollection<SimpleStatistics> simpleStatistics = new List<SimpleStatistics>
            {
                await GetSubcribersStatistics(eventSubsStatisticsTop),
                await GetMounthStatistics()
            };

            return simpleStatistics;
        }

        private async Task<SimpleStatistics> GetEventSubscribersByOrganizationStatistics(int eventId)
        {
            var eventEntity = await GetEventEntityAsync(eventId);

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
                    var organization = await _organizationService.GetOrganizationEntityAsync(entry.Key);
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
            if (level > 8)
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
                    subscribers.Add(await _userService.GetUserEntityAsync(subscription.ApplicationUserId));
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
                Title = $"Топ самых популярных событий по подписчикам",
                Data = new List<ChartDataPiece>()
            };

            var subscribersCountPerEvent = await _repositoryWrapper.Subscription
                .FindAll()
                .GroupBy(s => s.EventId)
                .Select(g => new { eventId = g.Key, count = g.Count() })
                .Take(number)
                .ToDictionaryAsync(k => k.eventId, i => i.count);

            foreach (var entry in subscribersCountPerEvent)
            {
                var eventEntity = await GetEventEntityAsync(entry.Key);

                statistics.Data.Add(
                    new ChartDataPiece
                    {
                        X = eventEntity.Name,
                        Y = entry.Value
                    });
            }

            return statistics;
        }

        private async Task<SimpleStatistics> GetMounthStatistics()
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
    }
}
