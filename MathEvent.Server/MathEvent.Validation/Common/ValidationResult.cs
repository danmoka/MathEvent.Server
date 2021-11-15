using MathEvent.Contracts.Validators;
using System.Collections.Generic;

namespace MathEvent.Validation.Common
{
    /// <summary>
    /// Описывает результат валидации
    /// </summary>
    public class ValidationResult : IValidationResult
    {
        public bool IsValid { get; set; }

        public IEnumerable<IValidationError> Errors { get; set; }
    }
}
