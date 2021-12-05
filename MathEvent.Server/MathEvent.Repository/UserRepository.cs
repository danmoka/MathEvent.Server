using MathEvent.Contracts;
using MathEvent.Database;
using MathEvent.Entities.Entities;
namespace MathEvent.Repository
{
    /// <summary>
    /// Репозиторий для Пользователей
    /// </summary>
    public class UserRepository : RepositoryBase<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
        }
    }
}
