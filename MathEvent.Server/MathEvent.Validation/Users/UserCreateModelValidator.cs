using MathEvent.Contracts.Validators;
using MathEvent.Models.Users;
using MathEvent.Validation.Common;
using MathEvent.Validation.Organization;
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

        private readonly OrganizationValidationUtils _organizationValidationUtils;

        public UserCreateModelValidator(
            UserValidationUtils userValidationUtils,
            OrganizationValidationUtils organizationValidationUtils)
        {
            _userValidationUtils = userValidationUtils;
            _organizationValidationUtils = organizationValidationUtils;
        }

        public async Task<IValidationResult> Validate(UserCreateModel model)
        {
            var validationErrors = new List<IValidationError>();

            validationErrors.AddRange(_userValidationUtils.ValidateName(model.Name));
            validationErrors.AddRange(_userValidationUtils.ValidateSurname(model.Surname));
            validationErrors.AddRange(await _userValidationUtils.ValidateEmail(model.Email));
            validationErrors.AddRange(_userValidationUtils.ValidateUsername(model.UserName));
            validationErrors.AddRange(_userValidationUtils.ValidatePassword(model.Password));

            if (model.Password != model.PasswordConfirm)
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(model.Password),
                    Message = "Пароли не совпадают"
                });
            }

            if (model.OrganizationId is not null)
            {
                validationErrors.AddRange(await _organizationValidationUtils.ValidateOrganizationId(model.OrganizationId));
            }

            return new ValidationResult
            {
                IsValid = validationErrors.Count < 1,
                Errors = validationErrors,
            };
        }
    }
}
