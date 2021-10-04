using System.Collections.Generic;

namespace MathEvent.Contracts.Validators
{
    /// <summary>
    /// Декларирует функциональность результата валидации
    /// </summary>
    public interface IValidationResult
    {
        bool IsValid { get; set; }

        IEnumerable<IValidationError> Errors { get; set; }
    }
}
