using MathEvent.Contracts;
using MathEvent.Entities;
using MathEvent.Entities.Entities;

namespace MathEvent.Repository
{
    public class SubscriptionRepository : RepositoryBase<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
