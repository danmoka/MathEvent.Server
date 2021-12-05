using MathEvent.Models.Others;
using MathEvent.Models.Users;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MathEvent.Contracts.Services
{
    /// <summary>
    /// Декларирует функциональность сервиса пользователей
    /// </summary>
    public interface IUserService
    {
        Task<IEnumerable<UserReadModel>> List(IDictionary<string, string> filters);

        Task<UserWithEventsReadModel> Retrieve(Guid id);

        Task<UserWithEventsReadModel> RetrieveByIdentityUserId(Guid identityUserId);

        Task<UserWithEventsReadModel> Create(UserCreateModel createModel);

        Task<UserWithEventsReadModel> UpdateByEmail(string email, UserUpdateModel updateModel);

        Task<UserWithEventsReadModel> UpdateByIdentityUserId(Guid identityUserId, UserUpdateModel updateModel);

        Task DeleteByIdentityUserId(Guid identityUserId);

        Task<UserWithEventsReadModel> GetUserByClaims(ClaimsPrincipal userPrincipal);

        Task<UserWithEventsReadModel> GetUserByEmail(string email);

        Task<UserWithEventsReadModel> GetUserByIdentityUserId(Guid identityUserId);

        Task<IEnumerable<ChartData>> GetUserStatistics(Guid identityUserId);

        Task<IEnumerable<ChartData>> GetUsersStatistics(IDictionary<string, string> filters);
    }
}
