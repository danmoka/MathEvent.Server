using MathEvent.Contracts;
using MathEvent.Entities;
using MathEvent.Entities.Models.Identities;

namespace MathEvent.Repository
{
    /// <summary>
    /// Репозиторий лоя Пользователей
    /// </summary>
    public class UserRepository : RepositoryBase<ApplicationUser>, IUserRepository
    {
        public UserRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
