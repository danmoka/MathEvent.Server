using MathEvent.Contracts;
using MathEvent.Database;
using MathEvent.Entities.Entities;

namespace MathEvent.Repository
{
    /// <summary>
    /// Репозиторий для Владельцев
    /// </summary>
    public class OwnerRepository : RepositoryBase<Owner>, IOwnerRepository
    {
        public OwnerRepository(ApplicationContext applicationContext) : base(applicationContext)
        {

        }
    }
}
