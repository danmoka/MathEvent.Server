using MathEvent.Contracts.Validators;
using MathEvent.Models.Users;
using MathEvent.Models.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Validation.Users
{
    /// <summary>
    /// Валидатор модели создания пользователя
    /// </summary>
    public class UserCreateModelValidator : IUserCreateModelValidator
    {
        private readonly UserValidationUtils _userValidationUtils;

        public UserCreateModelValidator(
            UserValidationUtils userValidationUtils)
        {
            _userValidationUtils = userValidationUtils;
        }

        public async Task<ValidationResult> Validate(UserCreateModel model)
        {
            var validationErrors = new List<ValidationError>();

            validationErrors.AddRange(await _userValidationUtils.ValidateIdentityUserId(model.IdentityUserId));
            validationErrors.AddRange(await _userValidationUtils.ValidateEmail(model.Email));
            validationErrors.AddRange(_userValidationUtils.ValidateName(model.Name));
            validationErrors.AddRange(_userValidationUtils.ValidateSurname(model.Surname));

            return new ValidationResult
            {
                IsValid = validationErrors.Count < 1,
                Errors = validationErrors,
            };
        }
    }
}
