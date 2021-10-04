using AutoMapper;
using MathEvent.Contracts;
using MathEvent.DTOs.Events;
using MathEvent.DTOs.Organizations;
using MathEvent.DTOs.Users;
using MathEvent.Entities.Entities;
using MathEvent.Models.Users;
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
                .ForMember(dest => dest.Events, opt => opt.MapFrom<IdToEventDTOResolver>())
                .ForMember(dest => dest.ManagedEvents, opt => opt.MapFrom<IdToMangedEventResolver>())
                .ForMember(dest => dest.Organization, opt => opt.MapFrom<IdToOrganizationDTOResolver>());

            // DTO -> Model
            CreateMap<UserDTO, UserReadModel>(); // чтение
            CreateMap<UserWithEventsDTO, UserWithEventsReadModel>(); // чтение
            CreateMap<UserWithEventsDTO, UserUpdateModel>() // обновление
                .ForMember(dest => dest.Events, opt => opt.MapFrom<EventDTOToIdResolver>())
                .ForMember(dest => dest.ManagedEvents, opt => opt.MapFrom<ManagedEventDTOToIdResolver>())
                .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom<OrganizationDTOToIdResolver>());

            // DTO -> Entity
            CreateMap<UserDTO, ApplicationUser>(); // создание
            CreateMap<UserWithEventsDTO, ApplicationUser>(); // обновление

            // Entity -> DTO
            CreateMap<ApplicationUser, UserWithEventsDTO>()
                .ForMember(dest => dest.Events, opt => opt.MapFrom<GetEventsDTOResolver>())
                .ForMember(dest => dest.Organization, opt => opt.MapFrom<GetOrganizationDTOResolver>())
                .ForMember(dest => dest.ManagedEvents, opt => opt.MapFrom<GetManagedEventsDTOResolver>());
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

        /// <summary>
        /// Класс, описывающий маппинг id события под управлением на transfer объект события
        /// </summary>
        public class IdToMangedEventResolver : IValueResolver<UserUpdateModel, UserWithEventsDTO, ICollection<EventDTO>>
        {
            private readonly IRepositoryWrapper _repositoryWrapper;
            private readonly IMapper _mapper;

            public IdToMangedEventResolver(IRepositoryWrapper repositoryWrapper, IMapper mapper)
            {
                _repositoryWrapper = repositoryWrapper;
                _mapper = mapper;
            }

            public ICollection<EventDTO> Resolve(UserUpdateModel source, UserWithEventsDTO destination, ICollection<EventDTO> destMember, ResolutionContext context)
            {
                var events = new HashSet<EventDTO>();

                foreach (var id in source.ManagedEvents)
                {
                    events.Add(_mapper.Map<EventDTO>(_repositoryWrapper.Event
                        .FindByCondition(ev => ev.Id == id)
                        .SingleOrDefault()));
                }

                return events;
            }
        }

        /// <summary>
        /// Класс, описывающий маппинг transfer объектов сущности события под управлением на id события
        /// </summary>
        public class ManagedEventDTOToIdResolver : IValueResolver<UserWithEventsDTO, UserUpdateModel, ICollection<int>>
        {
            public ICollection<int> Resolve(UserWithEventsDTO source, UserUpdateModel destination, ICollection<int> destMember, ResolutionContext context)
            {
                var ids = new HashSet<int>();

                foreach (var ev in source.ManagedEvents)
                {
                    ids.Add(ev.Id);
                }

                return ids;
            }
        }

        /// <summary>
        /// Класс, описывающий получение transfer объектов событий, связанных с управлением
        /// </summary>
        public class GetManagedEventsDTOResolver : IValueResolver<ApplicationUser, UserWithEventsDTO, ICollection<EventDTO>>
        {
            private readonly IRepositoryWrapper _repositoryWrapper;
            private readonly IMapper _mapper;

            public GetManagedEventsDTOResolver(IRepositoryWrapper repositoryWrapper, IMapper mapper)
            {
                _repositoryWrapper = repositoryWrapper;
                _mapper = mapper;
            }

            public ICollection<EventDTO> Resolve(ApplicationUser source, UserWithEventsDTO destination, ICollection<EventDTO> destMember, ResolutionContext context)
            {
                var events = new HashSet<EventDTO>();
                var management = _repositoryWrapper.Management
                    .FindByCondition(s => s.ApplicationUserId == source.Id)
                    .ToList();

                foreach (var value in management)
                {
                    events.Add(_mapper.Map<EventDTO>(_repositoryWrapper.Event
                        .FindByCondition(ev => ev.Id == value.EventId)
                        .SingleOrDefault()));
                }

                return events;
            }
        }

        /// <summary>
        /// Класс, описывающий маппинг transfer объекта организации на id организации
        /// </summary>
        public class OrganizationDTOToIdResolver : IValueResolver<UserWithEventsDTO, UserUpdateModel, int?>
        {
            public int? Resolve(UserWithEventsDTO source, UserUpdateModel destination, int? destMember, ResolutionContext context)
            {
                return source.Organization?.Id;
            }
        }

        /// <summary>
        /// Класс, описывающий получение transfer объекта организации, связанной с пользователем
        /// </summary>
        public class GetOrganizationDTOResolver : IValueResolver<ApplicationUser, UserWithEventsDTO, OrganizationDTO>
        {
            private readonly IRepositoryWrapper _repositoryWrapper;
            private readonly IMapper _mapper;

            public GetOrganizationDTOResolver(IRepositoryWrapper repositoryWrapper, IMapper mapper)
            {
                _repositoryWrapper = repositoryWrapper;
                _mapper = mapper;
            }

            public OrganizationDTO Resolve(ApplicationUser source, UserWithEventsDTO destination, OrganizationDTO destMember, ResolutionContext context)
            {
                return _mapper.Map<OrganizationDTO>(_repositoryWrapper.Organization
                        .FindByCondition(org => org.Id == source.OrganizationId)
                        .SingleOrDefault());
            }
        }

        /// <summary>
        /// Класс, описывающий маппинг id организации на transfer объект организации
        /// </summary>
        public class IdToOrganizationDTOResolver : IValueResolver<UserUpdateModel, UserWithEventsDTO, OrganizationDTO>
        {
            private readonly IRepositoryWrapper _repositoryWrapper;
            private readonly IMapper _mapper;

            public IdToOrganizationDTOResolver(IRepositoryWrapper repositoryWrapper, IMapper mapper)
            {
                _repositoryWrapper = repositoryWrapper;
                _mapper = mapper;
            }

            public OrganizationDTO Resolve(UserUpdateModel source, UserWithEventsDTO destination, OrganizationDTO destMember, ResolutionContext context)
            {
                return _mapper.Map<OrganizationDTO>(_repositoryWrapper.Organization
                         .FindByCondition(org => org.Id == source.OrganizationId)
                         .SingleOrDefault());
            }
        }
    }
}