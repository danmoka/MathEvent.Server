using MathEvent.Contracts.Validators;
using MathEvent.Models.Events;
using MathEvent.Models.Validation;
using MathEvent.Validation.Organization;
using MathEvent.Validation.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Validation.Events
{
    /// <summary>
    /// Валидатор модели обновления события
    /// </summary>
    public class EventUpdateModelValidator : IEventUpdateModelValidator
    {
        private readonly EventValidationUtils _eventValidationUtils;

        private readonly UserValidationUtils _userValidationUtils;

        private readonly OrganizationValidationUtils _organizationValidationUtils;

        public EventUpdateModelValidator(
            EventValidationUtils eventValidationUtils,
            UserValidationUtils userValidationUtils,
            OrganizationValidationUtils organizationValidationUtils)
        {
            _eventValidationUtils = eventValidationUtils;
            _userValidationUtils = userValidationUtils;
            _organizationValidationUtils = organizationValidationUtils;
        }

        public async Task<ValidationResult> Validate(EventUpdateModel model)
        {
            var validationErrors = new List<ValidationError>();

            validationErrors.AddRange(_eventValidationUtils.ValidateName(model.Name));
            validationErrors.AddRange(_eventValidationUtils.ValidateDescription(model.Description));
            validationErrors.AddRange(_eventValidationUtils.ValidateStartDateTime(model.StartDate));

            if (model.Location is not null)
            {
                validationErrors.AddRange(_eventValidationUtils.ValidateLocation(model.Location));
            }

            if (model.OrganizationId is not null)
            {
                validationErrors.AddRange(await _organizationValidationUtils.ValidateOrganizationId(model.OrganizationId));
            }

            validationErrors.AddRange(await _userValidationUtils.ValidateUserIds(model.ApplicationUsers, "Подписчик"));
            validationErrors.AddRange(await _userValidationUtils.ValidateUserIds(model.ApplicationUsers, "Менеджер"));

            if (model.Managers.Count < 1)
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(model.Managers),
                    Message = "Хотя бы один менеджер должен быть задан"
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
