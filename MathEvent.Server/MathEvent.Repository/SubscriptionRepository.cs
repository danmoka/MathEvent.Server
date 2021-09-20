using MathEvent.Contracts;
using MathEvent.Database;
using MathEvent.Entities.Entities;

namespace MathEvent.Repository
{
    public class SubscriptionRepository : RepositoryBase<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(ApplicationContext applicationContext) : base(applicationContext)
        {

        }
    }
}
