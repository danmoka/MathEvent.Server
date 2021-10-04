using MathEvent.Contracts.Validators;
using MathEvent.Models.Organizations;
using MathEvent.Validation.Common;
using MathEvent.Validation.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Validation.Organization
{
    /// <summary>
    /// Валидатор модели создания организации
    /// </summary>
    public class OrganizationCreateModelValidator : IOrganizationCreateModelValidator
    {
        private readonly OrganizationValidationUtils _organizationValidationUtils;

        private readonly UserValidationUtils _userValidationUtils;

        public OrganizationCreateModelValidator(
            OrganizationValidationUtils organizationValidationUtils,
            UserValidationUtils userValidationUtils)
        {
            _organizationValidationUtils = organizationValidationUtils;
            _userValidationUtils = userValidationUtils;
        }

        public async Task<IValidationResult> Validate(OrganizationCreateModel model)
        {
            var validationErrors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(model.ITN))
            {
                validationErrors.AddRange(_organizationValidationUtils.ValidateITN(model.ITN));
            }

            validationErrors.AddRange(_organizationValidationUtils.ValidateName(model.Name));
            validationErrors.AddRange(await _userValidationUtils.ValidateUserId(model.ManagerId));

            return new ValidationResult
            {
                IsValid = validationErrors.Count < 1,
                Errors = validationErrors,
            };
        }
    }
}
