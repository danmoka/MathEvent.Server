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
            CreateMap<Event, EventSimpleReadDTO>();
            CreateMap<EventCreateDTO, Event>();
            CreateMap<EventUpdateDTO, Event>()
                .ForMember(dest => dest.ApplicationUsers, opt => opt.MapFrom<IdToUserResolver>());
            CreateMap<Event, EventUpdateDTO>();
        }

        /// <summary>
        /// Класс, описывающий маппинг id пользователя на сущность пользователя
        /// </summary>
        public class IdToUserResolver : IValueResolver<EventUpdateDTO, Event, ICollection<ApplicationUser>>
        {
            private readonly IRepositoryWrapper _repositoryWrapper;
            public IdToUserResolver(IRepositoryWrapper repositoryWrapper)
            {
                _repositoryWrapper = repositoryWrapper;
            }

            public ICollection<ApplicationUser> Resolve(EventUpdateDTO source, Event destination, ICollection<ApplicationUser> destMember, ResolutionContext context)
            {
                var users = new HashSet<ApplicationUser>();

                foreach (var id in source.ApplicationUsers)
                {
                    users.Add(_repositoryWrapper.User
                        .FindByCondition(user => user.Id == id)
                        .SingleOrDefault());
                }

                return users;
            }
        }
    }
}
