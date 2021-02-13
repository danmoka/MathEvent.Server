using AutoMapper;
using MathEvent.Entities.Models.Identity.DTOs;

namespace MathEvent.Entities.Models.Identity.Profiles
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
