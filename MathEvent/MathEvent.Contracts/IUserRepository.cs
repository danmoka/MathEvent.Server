using MathEvent.Entities.Models.Identities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace MathEvent.Contracts
{
    /// <summary>
    /// Декларирует функциональность пользовательских репозиториев
    /// </summary>
    public interface IUserRepository : IRepositoryBase<ApplicationUser>
    {
        /// <summary>
        /// Декларирует создание пользователя с паролем
        /// </summary>
        /// <param name="entity">Пользователь</param>
        /// <param name="password">Пароль</param>
        /// <returns>Результат создания</returns>
        Task<IdentityResult> CreateAsync(ApplicationUser entity, string password);

        /// <summary>
        /// Декларирует обновление пользователя
        /// </summary>
        /// <param name="entity">Пользователь</param>
        /// <returns>Результат обновления</returns>
        Task<IdentityResult> UpdateAsync(ApplicationUser entity);

        /// <summary>
        /// Декларирует удаление пользователя
        /// </summary>
        /// <param name="entity">Пользователь</param>
        /// <returns>Результат удаления</returns>
        Task<IdentityResult> DeleteAsync(ApplicationUser entity);
    }
}
