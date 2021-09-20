using MathEvent.Contracts;
using MathEvent.Database;
using MathEvent.Entities.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace MathEvent.Repository
{
    /// <summary>
    /// Репозиторий для Пользователей
    /// </summary>
    public class UserRepository : RepositoryBase<ApplicationUser>, IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(
            ApplicationContext applicationContext,
            UserManager<ApplicationUser> userManager) : base(applicationContext)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Создает пользователя с паролем
        /// </summary>
        /// <param name="entity">Пользователь</param>
        /// <param name="password">Пароль</param>
        /// <returns>Результат создания пользователя</returns>
        public async Task<IdentityResult> CreateAsync(ApplicationUser entity, string password)
        {
            return await _userManager.CreateAsync(entity, password);
        }

        /// <summary>
        /// Обновляет пользователя
        /// </summary>
        /// <param name="entity">Пользователь</param>
        /// <returns>Результат обновления</returns>
        public async Task<IdentityResult> UpdateAsync(ApplicationUser entity)
        {
            return await _userManager.UpdateAsync(entity);
        }

        /// <summary>
        /// Удаляет пользователя
        /// </summary>
        /// <param name="entity">Пользователь</param>
        /// <returns>Результат удаления</returns>
        public async Task<IdentityResult> DeleteAsync(ApplicationUser entity)
        {
            return await _userManager.DeleteAsync(entity);
        }
    }
}
