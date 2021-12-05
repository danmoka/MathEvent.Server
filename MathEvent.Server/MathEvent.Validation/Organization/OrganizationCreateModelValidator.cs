using MathEvent.Contracts.Validators;
using MathEvent.Models.Organizations;
using MathEvent.Validation.Common;
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

        public OrganizationCreateModelValidator(
            OrganizationValidationUtils organizationValidationUtils)
        {
            _organizationValidationUtils = organizationValidationUtils;
        }

        public async Task<IValidationResult> Validate(OrganizationCreateModel model)
        {
            var validationErrors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(model.ITN))
            {
                validationErrors.AddRange(_organizationValidationUtils.ValidateITN(model.ITN));
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
