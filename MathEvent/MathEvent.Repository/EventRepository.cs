using MathEvent.Contracts;
using MathEvent.Entities;
using MathEvent.Entities.Models.Events;

namespace MathEvent.Repository
{
    /// <summary>
    /// Репозиторий для Событий
    /// </summary>
    public class EventRepository : RepositoryBase<Event>, IEventRepository
    {
        public EventRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
    }
}
