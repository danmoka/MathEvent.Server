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
        Task<IEnumerable<SR>> List(IDictionary<string, string> filters);

        Task<R> Retrieve(I id);

        Task<R> Create(C createModel);

        Task<R> Update(I id, U updateModel);

        Task Delete(I id);
    }
}
