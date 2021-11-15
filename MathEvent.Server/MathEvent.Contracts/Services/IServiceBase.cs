using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Contracts.Services
{
    /// <summary>
    /// Декларирует базовую функицональность сервиса
    /// </summary>
    /// <typeparam name="R">Модель чтения</typeparam>
    /// <typeparam name="SR">Модель чтения (упрощенная)</typeparam>
    /// <typeparam name="C">Модель создания</typeparam>
    /// <typeparam name="U">Модель обновления</typeparam>
    /// <typeparam name="I">Тип поля идентификатора</typeparam>
    public interface IServiceBase<R, SR, C, U, I>
    {
        Task<IEnumerable<SR>> ListAsync(IDictionary<string, string> filters);

        Task<R> RetrieveAsync(I id);

        Task<R> CreateAsync(C createModel);

        Task<R> UpdateAsync(I id, U updateModel);

        Task DeleteAsync(I id);
    }
}
