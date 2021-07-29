using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Events.DTOs;
using MathEvent.Converters.Events.Models;
using MathEvent.Converters.Identities.DTOs;
using MathEvent.Converters.Organizations.DTOs;
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
                .ForMember(dest => dest.ApplicationUsers, opt => opt.MapFrom<IdToUserDTOResolver>())
                .ForMember(dest => dest.Managers, opt => opt.MapFrom<IdToManagerDTOResolver>())
                .ForMember(dest => dest.Organization, opt => opt.MapFrom<IdToOrganizationDTOResolver>());

            // DTO -> Model
            CreateMap<EventDTO, EventSimpleReadModel>(); // чтение
            CreateMap<EventDTO, EventReadModel>(); // чтение
            CreateMap<EventWithUsersDTO, EventUpdateModel>()
                .ForMember(dest => dest.ApplicationUsers, opt => opt.MapFrom<UserDTOToIdResolver>())
                .ForMember(dest => dest.Managers, opt => opt.MapFrom<ManagerDTOToIdResolver>())
                .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom<OrganizationDTOToIdResolver>());// обновление
            CreateMap<EventWithUsersDTO, EventWithUsersReadModel>()
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom<FileOwnerIdResolver>()); // чтение

            // DTO -> Entity
            CreateMap<EventWithUsersDTO, Event>(); // обновление
            CreateMap<EventDTO, Event>(); // создание

            // Entity -> DTO
            CreateMap<Event, EventWithUsersDTO>()
                .ForMember(dest => dest.ApplicationUsers, opt => opt.MapFrom<GetUsersDTOResolver>())
                .ForMember(dest => dest.Managers, opt => opt.MapFrom<GetManagersDTOResolver>())
                .ForMember(dest => dest.Organization, opt => opt.MapFrom<GetOrganizationDTOResolver>());
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
        /// Класс, описывающий маппинг id менеджера на transfer объект менеджера
        /// </summary>
        public class IdToManagerDTOResolver : IValueResolver<EventUpdateModel, EventWithUsersDTO, ICollection<UserDTO>>
        {
            private readonly IRepositoryWrapper _repositoryWrapper;
            private readonly IMapper _mapper;

            public IdToManagerDTOResolver(IRepositoryWrapper repositoryWrapper, IMapper mapper)
            {
                _repositoryWrapper = repositoryWrapper;
                _mapper = mapper;
            }

            public ICollection<UserDTO> Resolve(EventUpdateModel source, EventWithUsersDTO destination, ICollection<UserDTO> destMember, ResolutionContext context)
            {
                var users = new HashSet<UserDTO>();

                foreach (var id in source.Managers)
                {
                    users.Add(_mapper.Map<UserDTO>(_repositoryWrapper.User
                        .FindByCondition(user => user.Id == id)
                        .SingleOrDefault()));
                }

                return users;
            }
        }

        /// <summary>
        /// Класс, описывающий маппинг transfer объектов сущности пользователя на id пользователя
        /// </summary>
        public class UserDTOToIdResolver : IValueResolver<EventWithUsersDTO, EventUpdateModel, ICollection<string>>
        {
            public ICollection<string> Resolve(EventWithUsersDTO source, EventUpdateModel destination, ICollection<string> destMember, ResolutionContext context)
            {
                var ids = new HashSet<string>();

                foreach (var user in source.ApplicationUsers)
                {
                    ids.Add(user.Id);
                }

                return ids;
            }
        }

        /// <summary>
        /// Класс, описывающий маппинг transfer объектов сущности менеджера на id менеджера
        /// </summary>
        public class ManagerDTOToIdResolver : IValueResolver<EventWithUsersDTO, EventUpdateModel, ICollection<string>>
        {
            public ICollection<string> Resolve(EventWithUsersDTO source, EventUpdateModel destination, ICollection<string> destMember, ResolutionContext context)
            {
                var ids = new HashSet<string>();

                foreach (var user in source.Managers)
                {
                    ids.Add(user.Id);
                }

                return ids;
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

        /// <summary>
        /// Класс, описывающий получение transfer объектов пользователей, связанных с управлением
        /// </summary>
        public class GetManagersDTOResolver : IValueResolver<Event, EventWithUsersDTO, ICollection<UserDTO>>
        {
            private readonly IRepositoryWrapper _repositoryWrapper;
            private readonly IMapper _mapper;

            public GetManagersDTOResolver(IRepositoryWrapper repositoryWrapper, IMapper mapper)
            {
                _repositoryWrapper = repositoryWrapper;
                _mapper = mapper;
            }

            public ICollection<UserDTO> Resolve(Event source, EventWithUsersDTO destination, ICollection<UserDTO> destMember, ResolutionContext context)
            {
                var users = new HashSet<UserDTO>();
                var managers = _repositoryWrapper.Management
                    .FindByCondition(s => s.EventId == source.Id)
                    .ToList();

                foreach (var manager in managers)
                {
                    users.Add(_mapper.Map<UserDTO>(_repositoryWrapper.User
                        .FindByCondition(user => user.Id == manager.ApplicationUserId)
                        .SingleOrDefault()));
                }

                return users;
            }
        }

        /// <summary>
        /// Класс, описывающий маппинг transfer объекта организации на id организации
        /// </summary>
        public class OrganizationDTOToIdResolver : IValueResolver<EventWithUsersDTO, EventUpdateModel, int?>
        {
            public int? Resolve(EventWithUsersDTO source, EventUpdateModel destination, int? destMember, ResolutionContext context)
            {
                return source.Organization?.Id;
            }
        }

        /// <summary>
        /// Класс, описывающий получение transfer объекта организации, связанной с событием
        /// </summary>
        public class GetOrganizationDTOResolver : IValueResolver<Event, EventWithUsersDTO, OrganizationDTO>
        {
            private readonly IRepositoryWrapper _repositoryWrapper;
            private readonly IMapper _mapper;

            public GetOrganizationDTOResolver(IRepositoryWrapper repositoryWrapper, IMapper mapper)
            {
                _repositoryWrapper = repositoryWrapper;
                _mapper = mapper;
            }

            public OrganizationDTO Resolve(Event source, EventWithUsersDTO destination, OrganizationDTO destMember, ResolutionContext context)
            {
                return _mapper.Map<OrganizationDTO>(_repositoryWrapper.Organization
                        .FindByCondition(org => org.Id == source.OrganizationId)
                        .SingleOrDefault());
            }
        }

        /// <summary>
        /// Класс, описывающий маппинг id организации на transfer объект организации
        /// </summary>
        public class IdToOrganizationDTOResolver : IValueResolver<EventUpdateModel, EventWithUsersDTO, OrganizationDTO>
        {
            private readonly IRepositoryWrapper _repositoryWrapper;
            private readonly IMapper _mapper;

            public IdToOrganizationDTOResolver(IRepositoryWrapper repositoryWrapper, IMapper mapper)
            {
                _repositoryWrapper = repositoryWrapper;
                _mapper = mapper;
            }

            public OrganizationDTO Resolve(EventUpdateModel source, EventWithUsersDTO destination, OrganizationDTO destMember, ResolutionContext context)
            {
                return _mapper.Map<OrganizationDTO>(_repositoryWrapper.Organization
                         .FindByCondition(org => org.Id == source.OrganizationId)
                         .SingleOrDefault());
            }
        }

        /// <summary>
        /// Класс, описывающий получение Id сущности владельца файла
        /// </summary>
        public class FileOwnerIdResolver : IValueResolver<EventWithUsersDTO, EventWithUsersReadModel, int?>
        {
            private readonly IRepositoryWrapper _repositoryWrapper;

            public FileOwnerIdResolver(IRepositoryWrapper repositoryWrapper)
            {
                _repositoryWrapper = repositoryWrapper;
            }

            public int? Resolve(EventWithUsersDTO source, EventWithUsersReadModel destination, int? destMember, ResolutionContext context)
            {
                var owner = _repositoryWrapper.Owner
                    .FindByCondition(ow => ow.EventId == source.Id && ow.OwnedType == Owner.Type.File)
                    .SingleOrDefault();

                if (owner is null)
                {
                    owner = CreateEventOwner(source.Id, Owner.Type.File);
                }

                return owner.Id;
            }

            /// <summary>
            /// Создает владельца-событие
            /// </summary>
            /// <param name="id">Идентификатор события</param>
            /// <param name="type">Тип обладаемой сущности</param>
            /// <returns>Владелец</returns>
            private Owner CreateEventOwner(int id, Owner.Type type)
            {
                var owner = _repositoryWrapper.Owner.CreateAsync(
                    new Owner
                    {
                        EventId = id,
                        OwnedType = type
                    }).Result;
                _repositoryWrapper.SaveAsync();

                return owner;
            }
        }
    }
}
