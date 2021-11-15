using MathEvent.Contracts.Validators;

namespace MathEvent.Validation.Common
{
    /// <summary>
    /// Описывает ошибку валидации
    /// </summary>
    public class ValidationError : IValidationError
    {
        public string Field { get; set; }

        public string Message { get; set; }
    }
}
