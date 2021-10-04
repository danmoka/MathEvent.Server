using MathEvent.Models.Events;
using MathEvent.Models.Files;
using MathEvent.Models.Others;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Contracts.Services
{
    /// <summary>
    /// Декларирует функциональность сервиса событий
    /// </summary>
    public interface IEventService : IServiceBase<
        EventWithUsersReadModel,
        EventReadModel,
        EventCreateModel,
        EventUpdateModel,
        int
        >
    {
        Task<EventWithUsersReadModel> UploadAvatar(int id, IFormFile file, FileCreateModel fileCreateModel);

        Task<IEnumerable<EventReadModel>> GetChildEvents(int id);

        Task<IEnumerable<EventWithUsersReadModel>> GetEventsByDate(DateTime start, DateTime end);

        Task<IEnumerable<Breadcrumb>> GetBreadcrumbs(int id);

        Task<IEnumerable<ChartData>> GetEventStatistics(int id);

        Task<IEnumerable<ChartData>> GetEventsStatistics(IDictionary<string, string> filters);

        Task<IDictionary<DateTime, int>> GetEventsCountByDateAsync(IDictionary<string, string> dates);
    }
}
