using System;
using System.ComponentModel.DataAnnotations;

namespace MathEvent.Converters.Events.Models.Validation
{
    /// <summary>
    /// Предназначен для описания нестандартной валидации отдельных полей
    /// </summary>
    internal static class ValidationUtils
    {
        internal static ValidationResult ValidateDateTime(DateTime time)
        {
            if (time.ToUniversalTime() < DateTime.UtcNow)
            {
                return new ValidationResult(
                    $"Дата начала события не должна быть меньше текущей",
                    new[] { nameof(time) });
            }

            return ValidationResult.Success;
        }
    }
}
