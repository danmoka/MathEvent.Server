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
    public class ForgotPasswordResetModelValidator : IForgotPasswordResetModelValidator
    {
        private readonly UserValidationUtils _userValidationUtils;

        public ForgotPasswordResetModelValidator(UserValidationUtils userValidationUtils)
        {
            _userValidationUtils = userValidationUtils;
        }

        public async Task<IValidationResult> Validate(ForgotPasswordResetModel model)
        {
            var validationErrors = new List<IValidationError>();

            if (string.IsNullOrEmpty(model.Token))
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(model.Token),
                    Message = "Токен должен быть задан"
                });
            }

            validationErrors.AddRange(await _userValidationUtils.ValidateEmail(model.Email, false));
            validationErrors.AddRange(_userValidationUtils.ValidatePassword(model.Password));

            if (model.Password != model.PasswordConfirm)
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(model.Password),
                    Message = "Пароли не совпадают"
                });
            }

            return new ValidationResult
            {
                IsValid = validationErrors.Count < 1,
                Errors = validationErrors,
            };
        }
    }
}
