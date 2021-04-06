using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Events.DTOs;
using MathEvent.Converters.Events.Models;
using MathEvent.Entities.Entities;
using MathEvent.Services.Messages;
using MathEvent.Services.Results;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public EventService(IRepositoryWrapper repositoryWrapper, IMapper mapper, IOwnerService ownerService)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _ownerService = ownerService;
        }

        public async Task<IEnumerable<EventReadModel>> ListAsync(IDictionary<string, string> filters)
        {
            var events = await Filter(_repositoryWrapper.Event.FindAll(), filters).ToListAsync();

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

        public async Task<AResult<IMessage, Event>> CreateAsync(EventCreateModel createModel)
        {
            var eventEntity = _mapper.Map<Event>(_mapper.Map<EventDTO>(createModel));

            if (eventEntity is null)
            {
                return new MessageResult<Event>
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
                return new MessageResult<Event>
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
                return new MessageResult<Event>
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

            return new MessageResult<Event>
            {
                Succeeded = true,
                Entity = eventEntityDb
            };
        }

        public async Task<AResult<IMessage, Event>> UpdateAsync(int id, EventUpdateModel updateModel)
        {
            var eventEntity = await GetEventEntityAsync(id);

            if (eventEntity is null)
            {
                return new MessageResult<Event>
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

            return new MessageResult<Event>
            {
                Succeeded = true,
                Entity = eventEntity
            };
        }

        public async Task<AResult<IMessage, Event>> DeleteAsync(int id)
        {
            var eventEntity = await GetEventEntityAsync(id);

            if (eventEntity is null)
            {
                return new MessageResult<Event>
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

            return new MessageResult<Event> { Succeeded = true };
        }

        public async Task<Event> GetEventEntityAsync(int id)
        {
            return await _repositoryWrapper.Event
                .FindByCondition(ev => ev.Id == id)
                .SingleOrDefaultAsync();
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
