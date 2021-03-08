using MathEvent.Contracts;
using MathEvent.Converters.Identities.Models;
using MathEvent.Entities.Entities;
using Service.Results;
using System.Threading.Tasks;

namespace Service.Services
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
    }
}
