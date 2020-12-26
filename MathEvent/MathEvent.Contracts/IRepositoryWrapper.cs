
using System.Threading.Tasks;

namespace MathEvent.Contracts
{
    public interface IRepositoryWrapper
    {
        IEventRepository Event { get; }
        Task SaveAsync();
    }
}
