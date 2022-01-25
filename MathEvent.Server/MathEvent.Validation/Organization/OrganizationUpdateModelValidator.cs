using MathEvent.Contracts.Validators;
using MathEvent.Models.Organizations;
using MathEvent.Models.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Validation.Organization
{
    /// <summary>
    /// Валидатор модели обновления организации
    /// </summary>
    public class OrganizationUpdateModelValidator : IOrganizationUpdateModelValidator
    {
        private readonly OrganizationValidationUtils _organizationValidationUtils;

        public OrganizationUpdateModelValidator(
            OrganizationValidationUtils organizationValidationUtils)
        {
            _organizationValidationUtils = organizationValidationUtils;
        }

        public async Task<ValidationResult> Validate(OrganizationUpdateModel model)
        {
            var validationErrors = new List<ValidationError>();

            if (!string.IsNullOrEmpty(model.ITN))
            {
                validationErrors.AddRange(await _organizationValidationUtils.ValidateITN(model.ITN));
            }

            if (!string.IsNullOrEmpty(model.Description))
            {
                validationErrors.AddRange(_organizationValidationUtils.ValidateDescription(model.Description));
            }

            validationErrors.AddRange(_organizationValidationUtils.ValidateName(model.Name));

            return new ValidationResult
            {
                IsValid = validationErrors.Count < 1,
                Errors = validationErrors,
            };
        }
    }
}
