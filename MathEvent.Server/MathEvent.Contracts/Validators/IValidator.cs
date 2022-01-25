using MathEvent.Models.Validation;
using System.Threading.Tasks;

namespace MathEvent.Contracts.Validators
{
    /// <summary>
    /// Декларирует функциональность валидатора
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    public interface IValidator<T> where T : class
    {
        Task<ValidationResult> Validate(T obj);
    }
}
