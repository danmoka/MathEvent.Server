using MathEvent.Contracts;
using MathEvent.Database;
using MathEvent.Entities.Entities;

namespace MathEvent.Repository
{
    public class OrganizationRepository : RepositoryBase<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
        }
    }
}
