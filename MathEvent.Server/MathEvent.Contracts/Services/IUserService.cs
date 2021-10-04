using MathEvent.Entities.Entities;
using MathEvent.Models.Others;
using MathEvent.Models.Users;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MathEvent.Contracts.Services
{
    /// <summary>
    /// Декларирует функциональность сервиса пользователей
    /// </summary>
    public interface IUserService : IServiceBase<
        UserWithEventsReadModel,
        UserReadModel,
        UserCreateModel,
        UserUpdateModel,
        string
        >
    {
        Task<ApplicationUser> GetUserAsync(ClaimsPrincipal userPrincipal);

        Task<ApplicationUser> GetUserByEmail(string email);

        Task ForgotPasswordAsync(string email);

        Task ResetPasswordAsync(ForgotPasswordResetModel resetModel);

        Task<IEnumerable<ChartData>> GetUserStatistics(string id);

        Task<IEnumerable<ChartData>> GetUsersStatistics(IDictionary<string, string> filters);
    }
}
