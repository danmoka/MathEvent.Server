using MathEvent.Contracts;
using MathEvent.Converters.Events.Models;
using MathEvent.Converters.Files.Models;
using MathEvent.Converters.Others;
using MathEvent.Entities.Entities;
using MathEvent.Services.Results;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Services.Services
{
    /// <summary>
    /// Декларирует функциональность сервисов событий
    /// </summary>
    public interface IEventService : IControllerService<
        EventReadModel,
        EventWithUsersReadModel,
        EventCreateModel,
        EventUpdateModel,
        int,
        AResult<IMessage, EventWithUsersReadModel>>
    {
        Task<Event> GetEventEntityAsync(int id);
        Task<AResult<IMessage, IEnumerable<Breadcrumb>>> GetBreadcrumbs(int id);
        Task<IEnumerable<SimpleStatistics>> GetSimpleStatistics(IDictionary<string, string> filters);
        Task<IEnumerable<SimpleStatistics>> GetEventStatistics(int id);
        Task<AResult<IMessage, EventWithUsersReadModel>> UploadAvatar(int id, IFormFile file, FileCreateModel fileCreateModel);
    }
}
