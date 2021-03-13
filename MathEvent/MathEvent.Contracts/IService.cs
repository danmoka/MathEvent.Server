using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Contracts
{
    /// <summary>
    /// Декларирует функциональность сервиса
    /// </summary>
    /// <typeparam name="LR">Тип модели для чтения списка сущностей (List Read)</typeparam>
    /// <typeparam name="RR">Тип модели для чтения конкретной сущности (Retrieve Read)</typeparam>
    /// <typeparam name="C">Тип модели для создания сущности (Create)</typeparam>
    /// <typeparam name="U">Тип модели для изменения сущности (Update)</typeparam>
    /// <typeparam name="I">Тип первичного ключа (Id)</typeparam>
    /// <typeparam name="R">Тип результата выполнения (Result)</typeparam>
    public interface IService<LR, RR, C, U, I, R>
    {
        public Task<IEnumerable<LR>> ListAsync(IDictionary<string, string> filters);

        public Task<RR> RetrieveAsync(I id);

        public Task<R> CreateAsync(C createModel);

        public Task<R> UpdateAsync(I id, U updateModel);

        public Task<R> DeleteAsync(I id);
    }
}
