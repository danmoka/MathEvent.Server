using AutoMapper;
using MathEvent.Entities.Models.Event.DTOs;
using MathEvent.Entities.Models.Identity;
using System.Collections.Generic;
using System.Linq;

namespace MathEvent.Entities.Models.Event.Profiles
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
        private class CustomResolver : IValueResolver<EventUpdateDTO, Event, ICollection<ApplicationUser>>
        {
            readonly RepositoryContext _repositoryContext;
            public CustomResolver(RepositoryContext repositoryContext)
            {
                _repositoryContext = repositoryContext;
            }

            public ICollection<ApplicationUser> Resolve(EventUpdateDTO source, Event destination, ICollection<ApplicationUser> destMember, ResolutionContext context)
            {
                var users = new HashSet<ApplicationUser>();

                foreach (var id in source.ApplicationUsers)
                {
                    users.Add(_repositoryContext.ApplicationUsers.Where(x => x.Id == id).SingleOrDefault());
                }

                return users;
            }
        }
    }
}
