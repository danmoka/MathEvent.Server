using AutoMapper;
using MathEvent.Converters.Identities.DTOs;
using MathEvent.Entities.Models.Identities;

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
        }
    }
}
