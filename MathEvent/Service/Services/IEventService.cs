using MathEvent.Contracts;
using MathEvent.Converters.Events.Models;
using MathEvent.Entities.Entities;
using Service.Results;
using System.Threading.Tasks;

namespace MathEvent.Service.Services
{
    /// <summary>
    /// Декларирует функциональность сервисов событий
    /// </summary>
    public interface IEventService : IService<
        EventReadModel,
        EventWithUsersReadModel,
        EventCreateModel,
        EventUpdateModel,
        int,
        AResult<IMessage, Event>>
    {
        Task<Event> GetEventEntityAsync(int id);
    }
}
