using MathEvent.Contracts;
using MathEvent.Database;
using MathEvent.Entities.Entities;

namespace MathEvent.Repository
{
    /// <summary>
    /// Репозиторий для Событий
    /// </summary>
    public class EventRepository : RepositoryBase<Event>, IEventRepository
    {
        public EventRepository(ApplicationContext applicationContext) : base(applicationContext)
        {

        }
    }
}
