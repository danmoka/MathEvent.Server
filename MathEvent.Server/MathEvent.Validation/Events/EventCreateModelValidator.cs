using MathEvent.Contracts.Validators;
using MathEvent.Models.Events;
using MathEvent.Validation.Common;
using MathEvent.Validation.Organization;
using MathEvent.Validation.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Validation.Events
{
    /// <summary>
    /// Валидатор модели создания события
    /// </summary>
    public class EventCreateModelValidator : IEventCreateModelValidator
    {
        private readonly EventValidationUtils _eventValidationUtils;

        private readonly UserValidationUtils _userValidationUtils;

        private readonly OrganizationValidationUtils _organizationValidationUtils;

        public EventCreateModelValidator(
            EventValidationUtils eventValidationUtils,
            UserValidationUtils userValidationUtils,
            OrganizationValidationUtils organizationValidationUtils)
        {
            _eventValidationUtils = eventValidationUtils;
            _userValidationUtils = userValidationUtils;
            _organizationValidationUtils = organizationValidationUtils;
        }

        public async Task<IValidationResult> Validate(EventCreateModel model)
        {
            var validationErrors = new List<IValidationError>();

            validationErrors.AddRange(_eventValidationUtils.ValidateName(model.Name));
            validationErrors.AddRange(_eventValidationUtils.ValidateDescription(model.Description));
            validationErrors.AddRange(_eventValidationUtils.ValidateStartDateTime(model.StartDate));

            if (model.Location is not null)
            {
                validationErrors.AddRange(_eventValidationUtils.ValidateLocation(model.Location));
            }

            if (model.AuthorId is not null)
            {
                validationErrors.AddRange(await _userValidationUtils.ValidateUserId(model.AuthorId));
            }

            if (model.OrganizationId is not null)
            {
                validationErrors.AddRange(await _organizationValidationUtils.ValidateOrganizationId(model.OrganizationId));
            }

            if (model.ParentId is not null)
            {
                var validationParentFieldErrors = await _eventValidationUtils.ValidateParentEventId(model.ParentId);

                validationErrors.AddRange(validationParentFieldErrors);
            }

            return new ValidationResult
            {
                IsValid = validationErrors.Count < 1,
                Errors = validationErrors,
            };
        }
    }
}
