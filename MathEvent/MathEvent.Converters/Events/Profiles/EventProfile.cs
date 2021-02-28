using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Events.DTOs;
using MathEvent.Converters.Events.Models;
using MathEvent.Converters.Identities.DTOs;
using MathEvent.Entities.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MathEvent.Converters.Events.Profiles
{
    /// <summary>
    /// Класс для связи с моделью БЛ
    /// </summary>
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            //Source -> target

            // Model -> DTO
            CreateMap<EventCreateModel, EventDTO>(); // создание
            CreateMap<EventUpdateModel, EventWithUsersDTO>() // обновление
                .ForMember(dest => dest.ApplicationUsers, opt => opt.MapFrom<IdToUserDTOResolver>());

            // DTO -> Model
            CreateMap<EventDTO, EventSimpleReadModel>(); // чтение
            CreateMap<EventDTO, EventReadModel>(); // чтение
            CreateMap<EventWithUsersDTO, EventUpdateModel>(); // обновление
            CreateMap<EventWithUsersDTO, EventWithUsersReadModel>(); // чтение

            // DTO -> Entity
            CreateMap<EventWithUsersDTO, Event>(); // обновление
            CreateMap<EventDTO, Event>(); // создание

            // Entity -> DTO
            CreateMap<Event, EventWithUsersDTO>()
                .ForMember(dest => dest.ApplicationUsers, opt => opt.MapFrom<GetUsersDTOResolver>());
            CreateMap<Event, EventDTO>();
        }

        /// <summary>
        /// Класс, описывающий маппинг id пользователя на transfer объект пользователя
        /// </summary>
        public class IdToUserDTOResolver : IValueResolver<EventUpdateModel, EventWithUsersDTO, ICollection<UserDTO>>
        {
            private readonly IRepositoryWrapper _repositoryWrapper;
            private readonly IMapper _mapper;

            public IdToUserDTOResolver(IRepositoryWrapper repositoryWrapper, IMapper mapper)
            {
                _repositoryWrapper = repositoryWrapper;
                _mapper = mapper;
            }

            public ICollection<UserDTO> Resolve(EventUpdateModel source, EventWithUsersDTO destination, ICollection<UserDTO> destMember, ResolutionContext context)
            {
                var users = new HashSet<UserDTO>();

                foreach (var id in source.ApplicationUsers)
                {
                    users.Add(_mapper.Map<UserDTO>(_repositoryWrapper.User
                        .FindByCondition(user => user.Id == id)
                        .SingleOrDefault()));
                }

                return users;
            }
        }

        /// <summary>
        /// Класс, описывающий получение transfer объектов пользователей, связанных с событием
        /// </summary>
        public class GetUsersDTOResolver : IValueResolver<Event, EventWithUsersDTO, ICollection<UserDTO>>
        {
            private readonly IRepositoryWrapper _repositoryWrapper;
            private readonly IMapper _mapper;

            public GetUsersDTOResolver(IRepositoryWrapper repositoryWrapper, IMapper mapper)
            {
                _repositoryWrapper = repositoryWrapper;
                _mapper = mapper;
            }

            public ICollection<UserDTO> Resolve(Event source, EventWithUsersDTO destination, ICollection<UserDTO> destMember, ResolutionContext context)
            {
                var users = new HashSet<UserDTO>();
                var subscriptions = _repositoryWrapper.Subscription
                    .FindByCondition(s => s.EventId == source.Id)
                    .ToList();

                foreach (var subscription in subscriptions)
                {
                    users.Add(_mapper.Map<UserDTO>(_repositoryWrapper.User
                        .FindByCondition(user => user.Id == subscription.ApplicationUserId)
                        .SingleOrDefault()));
                }

                return users;
            }
        }
    }
}
