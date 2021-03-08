using System.Threading.Tasks;

namespace MathEvent.Contracts
{
    /// <summary>
    /// Декларирует функциональность оберток репозиториев
    /// </summary>
    public interface IRepositoryWrapper
    {
        IEventRepository Event { get; }

        IUserRepository User { get; }

        ISubscriptionRepository Subscription { get; }

        IManagerRepository Manager { get; }

        Task SaveAsync();
    }
}
