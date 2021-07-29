using MathEvent.Contracts;
using MathEvent.Entities.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace MathEvent.Services.Services
{
    /// <summary>
    /// Декларирует функциональность сервисов владельцев
    /// </summary>
    public class OwnerService : IOwnerService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        public OwnerService(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }

        /// <summary>
        /// Создает владельца-событие
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        /// <param name="type">Тип обладаемой сущности</param>
        /// <returns>Владелец</returns>
        public async Task<Owner> CreateEventOwnerAsync(int id, Owner.Type type)
        {
            var owner = await _repositoryWrapper.Owner.CreateAsync(
                new Owner
                {
                    EventId = id,
                    OwnedType = type
                });
            await _repositoryWrapper.SaveAsync();

            return owner;
        }

        /// <summary>
        /// Создает владельца-пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="type">Тип обладаемой сущности</param>
        /// <returns>Владелец</returns>
        public async Task<Owner> CreateUserOwnerAsync(string id, Owner.Type type)
        {
            var owner = await _repositoryWrapper.Owner.CreateAsync(
                new Owner
                {
                    ApplicationUserId = id,
                    OwnedType = type
                });
            await _repositoryWrapper.SaveAsync();

            return owner;
        }

        public async Task<Owner> GetEventOwnerAsync(int id, Owner.Type type)
        {
            var owner = _repositoryWrapper.Owner
                    .FindByCondition(ow => ow.EventId == id && ow.OwnedType == type)
                    .SingleOrDefault();

            if (owner is null)
            {
                owner = await CreateEventOwnerAsync(id, type);
            }

            return owner;
        }

        public async Task<Owner> GetUserOwnerAsync(string id, Owner.Type type)
        {
            var owner = _repositoryWrapper.Owner
                    .FindByCondition(ow => ow.ApplicationUserId == id && ow.OwnedType == type)
                    .SingleOrDefault();

            if (owner is null)
            {
                owner = await CreateUserOwnerAsync(id, type);
            }

            return owner;
        }
    }

    public interface IOwnerService
    {
        Task<Owner> CreateEventOwnerAsync(int id, Owner.Type type);

        Task<Owner> CreateUserOwnerAsync(string id, Owner.Type type);

        Task<Owner> GetEventOwnerAsync(int id, Owner.Type type);

        Task<Owner> GetUserOwnerAsync(string id, Owner.Type type);
    }
}
