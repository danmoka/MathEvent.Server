using MathEvent.Contracts;
using MathEvent.Database;
using System.Threading.Tasks;

namespace MathEvent.Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private RepositoryContext _repositoryContext;
        private IEventRepository _event;

        public RepositoryWrapper(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public IEventRepository Event
        {
            get
            {
                if (_event is null)
                {
                    _event = new EventRepository(_repositoryContext);
                }

                return _event;
            }
        }

        public async Task SaveAsync()
        {
            await _repositoryContext.SaveChangesAsync();
        }
    }
}
