using MathEvent.Contracts;
using MathEvent.Database;
using MathEvent.Entities.Models.Event;

namespace MathEvent.Repository
{
    public class EventRepository : RepositoryBase<Event>, IEventRepository
    {
        public EventRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {

        }
    }
}
