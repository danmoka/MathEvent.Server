using MathEvent.Contracts;
using MathEvent.Entities;
using MathEvent.Entities.Entities;

namespace MathEvent.Repository
{
    /// <summary>
    /// Репозиторий для Менеджеров
    /// </summary>
    public class ManagementRepository : RepositoryBase<Management>, IManagementRepository
    {
        public ManagementRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
