using MathEvent.Contracts;
using MathEvent.Database;
using MathEvent.Entities.Entities;

namespace MathEvent.Repository
{
    /// <summary>
    /// Репозиторий для Менеджеров
    /// </summary>
    public class ManagementRepository : RepositoryBase<Management>, IManagementRepository
    {
        public ManagementRepository(ApplicationContext applicationContext) : base(applicationContext)
        {

        }
    }
}
