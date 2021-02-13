using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Events.DTOs;
using MathEvent.Entities.Models.Events;
using MathEvent.Entities.Models.Identities;
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
            CreateMap<Event, EventReadDTO>();
            CreateMap<EventCreateDTO, Event>();
            CreateMap<EventUpdateDTO, Event>()
                .ForMember(dest => dest.ApplicationUsers, opt => opt.MapFrom<CustomResolver>());
            CreateMap<Event, EventUpdateDTO>();
        }

        /// <summary>
        /// Класс, описывающий маппинг id пользователя на сущность пользователя
        /// </summary>
        public class CustomResolver : IValueResolver<EventUpdateDTO, Event, ICollection<ApplicationUser>>
        {
            readonly IRepositoryWrapper _repositoryWrapper;
            public CustomResolver(IRepositoryWrapper repositoryWrapper)
            {
                _repositoryWrapper = repositoryWrapper;
            }

            public ICollection<ApplicationUser> Resolve(EventUpdateDTO source, Event destination, ICollection<ApplicationUser> destMember, ResolutionContext context)
            {
                var users = new HashSet<ApplicationUser>();

                foreach (var id in source.ApplicationUsers)
                {
                    users.Add(_repositoryWrapper.User.FindByCondition(x => x.Id == id).SingleOrDefault());
                }

                return users;
            }
        }
    }
}
