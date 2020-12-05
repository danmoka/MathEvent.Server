using AutoMapper;
using MathEventWebApi.Dtos;
using MathEventWebApi.Models;

namespace MathEventWebApi.Profiles
{
    // Класс для связи модели и сериализатора
    public class EventsProfile : Profile
    {
        public EventsProfile()
        {
            //Source -> target
            CreateMap<Event, EventReadDto>();
            CreateMap<EventCreateDto, Event>();
            CreateMap<EventUpdateDto, Event>();
            CreateMap<Event, EventUpdateDto>();
        }
    }
}