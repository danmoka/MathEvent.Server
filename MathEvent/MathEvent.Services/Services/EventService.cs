using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Events.DTOs;
using MathEvent.Converters.Events.Models;
using MathEvent.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Service.Messages;
using Service.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Service.Services
{
    /// <summary>
    /// Сервис по выполнению действий над событиями
    /// </summary>
    public class EventService : IEventService
    {
        private IRepositoryWrapper _repositoryWrapper { get; set; }

        private IMapper _mapper { get; set; }

        public EventService(IRepositoryWrapper repositoryWrapper, IMapper mapper)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EventReadModel>> ListAsync()
        {
            var events = await _repositoryWrapper.Event
                .FindAll()
                .ToListAsync();

            if (events != null)
            {
                var eventsDTO = _mapper.Map<IEnumerable<EventDTO>>(events);

                return _mapper.Map<IEnumerable<EventReadModel>>(eventsDTO);
            }

            return null;
        }

        public async Task<EventWithUsersReadModel> RetrieveAsync(int id)
        {
            var eventEntity = await GetEventEntityAsync(id);

            if (eventEntity != null)
            {
                var eventDTO = _mapper.Map<EventWithUsersDTO>(eventEntity);

                return _mapper.Map<EventWithUsersReadModel>(eventDTO);
            }

            return null;
        }

        public async Task<AResult<IMessage, Event>> CreateAsync(EventCreateModel createModel)
        {
            var eventEntity = _mapper.Map<Event>(_mapper.Map<EventDTO>(createModel));

            if (eventEntity is not null)
            {
                await _repositoryWrapper.Event.CreateAsync(eventEntity);
                await _repositoryWrapper.SaveAsync();

                return new MessageResult<Event>
                {
                    Succeeded = true,
                    Entity = eventEntity
                };
            }

            return new MessageResult<Event>
            {
                Succeeded = false,
                Messages = new List<SimpleMessage>
                {
                    new SimpleMessage
                    {
                        Message = "Errors when mapping model"
                    }
                }
            };
        }

        public async Task<AResult<IMessage, Event>> UpdateAsync(int id, EventUpdateModel updateModel)
        {
            var eventEntity = await GetEventEntityAsync(id);

            if (eventEntity is not null)
            {
                await CreateNewSubscriptions(updateModel.ApplicationUsers, id);
                var eventDTO = _mapper.Map<EventWithUsersDTO>(eventEntity);
                _mapper.Map(updateModel, eventDTO);
                _mapper.Map(eventDTO, eventEntity);
                _repositoryWrapper.Event.Update(eventEntity);
                await _repositoryWrapper.SaveAsync();

                return new MessageResult<Event> { Succeeded = true };
            }

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

        public async Task<AResult<IMessage, Event>> DeleteAsync(int id)
        {
            var eventEntity = await GetEventEntityAsync(id);

            if (eventEntity is not null)
            {
                _repositoryWrapper.Event.Delete(eventEntity);
                await _repositoryWrapper.SaveAsync();

                return new MessageResult<Event> { Succeeded = true };
            }

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
    }
}
