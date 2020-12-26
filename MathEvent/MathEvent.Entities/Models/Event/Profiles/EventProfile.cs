using AutoMapper;
using MathEvent.Entities.Models.Event.DTOs;

namespace MathEvent.Entities.Models.Event.Profiles
{
    public class EventProfile : Profile
    {
        /// <summary>
        /// Класс для связи с моделью БЛ
        /// </summary>
        public EventProfile()
        {
            //Source -> target
            CreateMap<Event, EventReadDTO>();
            CreateMap<EventCreateDTO, Event>();
            CreateMap<EventUpdateDTO, Event>();
            CreateMap<Event, EventUpdateDTO>();
        }
    }
}
