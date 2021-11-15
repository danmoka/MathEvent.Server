using MathEvent.Contracts.Validators;
using MathEvent.Models.Users;
using MathEvent.Validation.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Validation.Users
{
    /// <summary>
    /// Валидатор модели смены пароля
    /// </summary>
    public class ForgotPasswordModelValidator : IForgotPasswordModelValidator
    {
        private readonly UserValidationUtils _userValidationUtils;

        public ForgotPasswordModelValidator(UserValidationUtils userValidationUtils)
        {
            _userValidationUtils = userValidationUtils;
        }

        public async Task<IValidationResult> Validate(ForgotPasswordModel model)
        {
            var validationErrors = new List<IValidationError>();
            validationErrors.AddRange(await _userValidationUtils.ValidateEmail(model.Email, false));

            return new ValidationResult
            {
                IsValid = validationErrors.Count < 1,
                Errors = validationErrors,
            };
        }
    }
}
