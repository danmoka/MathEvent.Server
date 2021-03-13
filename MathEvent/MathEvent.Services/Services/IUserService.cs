using MathEvent.Contracts;
using MathEvent.Converters.Identities.Models;
using MathEvent.Entities.Entities;
using MathEvent.Services.Results;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MathEvent.Services.Services
{
    /// <summary>
    /// Декларирует функциональность сервисов пользователей
    /// </summary>
    public interface IUserService : IService<
        UserReadModel,
        UserWithEventsReadModel,
        UserCreateModel,
        UserUpdateModel,
        string,
        AResult<IMessage, ApplicationUser>>
    {
        Task<ApplicationUser> GetUserEntityAsync(string id);
        Task<ApplicationUser> GetCurrentUserAsync(ClaimsPrincipal user);
    }
}
