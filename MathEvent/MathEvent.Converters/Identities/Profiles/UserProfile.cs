using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Identities.DTOs;
using MathEvent.Entities.Models.Events;
using MathEvent.Entities.Models.Identities;
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
            CreateMap<ApplicationUser, UserReadDTO>();
            CreateMap<ApplicationUser, UserSimpleReadDTO>();
            CreateMap<ApplicationUser, UserWithEventsReadDTO>();
            CreateMap<UserUpdateDTO, ApplicationUser>()
                .ForMember(dest => dest.Events, opt => opt.MapFrom<IdToEventResolver>())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
            CreateMap<ApplicationUser, UserUpdateDTO>()
                .ForMember(dest => dest.Events, opt => opt.MapFrom<EventToIdResolver>());
            CreateMap<UserCreateDTO, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
        }

        /// <summary>
        /// Класс, описывающий маппинг id события на сущность события
        /// </summary>
        public class IdToEventResolver : IValueResolver<UserUpdateDTO, ApplicationUser, ICollection<Event>>
        {
            private readonly IRepositoryWrapper _repositoryWrapper;
            public IdToEventResolver(IRepositoryWrapper repositoryWrapper)
            {
                _repositoryWrapper = repositoryWrapper;
            }

            public ICollection<Event> Resolve(UserUpdateDTO source, ApplicationUser destination, ICollection<Event> destMember, ResolutionContext context)
            {
                var events = new HashSet<Event>();

                foreach (var id in source.Events)
                {
                    events.Add(_repositoryWrapper.Event
                        .FindByCondition(ev => ev.Id == id)
                        .SingleOrDefault());
                }

                return events;
            }
        }

        /// <summary>
        /// Класс, описывающий маппинг сущности события на id события
        /// </summary>
        public class EventToIdResolver : IValueResolver<ApplicationUser, UserUpdateDTO, ICollection<int>>
        {
            public ICollection<int> Resolve(ApplicationUser source, UserUpdateDTO destination, ICollection<int> destMember, ResolutionContext context)
            {
                var ids = new HashSet<int>();

                foreach (var ev in source.Events)
                {
                    ids.Add(ev.Id);
                }

                return ids;
            }
        }
    }
}
