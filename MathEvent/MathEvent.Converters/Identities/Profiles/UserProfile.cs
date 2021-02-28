using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Events.DTOs;
using MathEvent.Converters.Identities.DTOs;
using MathEvent.Converters.Identities.Models;
using MathEvent.Entities.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MathEvent.Converters.Identities.Profiles
{
    public class UserProfile : Profile
    {
        /// <summary>
        /// Класс для связи с моделью БЛ
        /// </summary>
        public UserProfile()
        {
            // Source -> target

            // Model -> DTO
            CreateMap<UserCreateModel, UserDTO>(); // создание
            CreateMap<UserUpdateModel, UserWithEventsDTO>() // обновление
                .ForMember(dest => dest.Events, opt => opt.MapFrom<IdToEventDTOResolver>());

            // DTO -> Model
            CreateMap<UserDTO, UserReadModel>(); // чтение
            CreateMap<UserDTO, UserSimpleReadModel>(); // чтение
            CreateMap<UserWithEventsDTO, UserWithEventsReadModel>(); // чтение
            CreateMap<UserWithEventsDTO, UserUpdateModel>() // обновление
                .ForMember(dest => dest.Events, opt => opt.MapFrom<EventDTOToIdResolver>());

            // DTO -> Entity
            CreateMap<UserDTO, ApplicationUser>(); // создание
            CreateMap<UserWithEventsDTO, ApplicationUser>(); // обновление

            // Entity -> DTO
            CreateMap<ApplicationUser, UserWithEventsDTO>()
                .ForMember(dest => dest.Events, opt => opt.MapFrom<GetEventsDTOResolver>());
            CreateMap<ApplicationUser, UserDTO>();
        }

        /// <summary>
        /// Класс, описывающий маппинг id события на трансфер объект сущности события
        /// </summary>
        public class IdToEventDTOResolver : IValueResolver<UserUpdateModel, UserWithEventsDTO, ICollection<EventDTO>>
        {
            private readonly IRepositoryWrapper _repositoryWrapper;
            private readonly IMapper _mapper;

            public IdToEventDTOResolver(IRepositoryWrapper repositoryWrapper, IMapper mapper)
            {
                _repositoryWrapper = repositoryWrapper;
                _mapper = mapper;
            }

            public ICollection<EventDTO> Resolve(UserUpdateModel source, UserWithEventsDTO destination, ICollection<EventDTO> destMember, ResolutionContext context)
            {
                var events = new HashSet<EventDTO>();

                foreach (var id in source.Events)
                {
                    events.Add(_mapper.Map<EventDTO>(_repositoryWrapper.Event
                        .FindByCondition(ev => ev.Id == id)
                        .SingleOrDefault()));
                }

                return events;
            }
        }

        /// <summary>
        /// Класс, описывающий маппинг transfer объектов сущности события на id события
        /// </summary>
        public class EventDTOToIdResolver : IValueResolver<UserWithEventsDTO, UserUpdateModel, ICollection<int>>
        {
            public ICollection<int> Resolve(UserWithEventsDTO source, UserUpdateModel destination, ICollection<int> destMember, ResolutionContext context)
            {
                var ids = new HashSet<int>();

                foreach (var ev in source.Events)
                {
                    ids.Add(ev.Id);
                }

                return ids;
            }
        }

        /// <summary>
        /// Класс, описывающий получение transfer объектов событий, связанных с пользователем
        /// </summary>
        public class GetEventsDTOResolver : IValueResolver<ApplicationUser, UserWithEventsDTO, ICollection<EventDTO>>
        {
            private readonly IRepositoryWrapper _repositoryWrapper;
            private readonly IMapper _mapper;

            public GetEventsDTOResolver(IRepositoryWrapper repositoryWrapper, IMapper mapper)
            {
                _repositoryWrapper = repositoryWrapper;
                _mapper = mapper;
            }

            public ICollection<EventDTO> Resolve(ApplicationUser source, UserWithEventsDTO destination, ICollection<EventDTO> destMember, ResolutionContext context)
            {
                var events = new HashSet<EventDTO>();
                var subscriptions = _repositoryWrapper.Subscription
                    .FindByCondition(s => s.ApplicationUserId == source.Id)
                    .ToList();

                foreach (var subscription in subscriptions)
                {
                    events.Add(_mapper.Map<EventDTO>(_repositoryWrapper.Event
                        .FindByCondition(ev => ev.Id == subscription.EventId)
                        .SingleOrDefault()));
                }

                return events;
            }
        }
    }
}
