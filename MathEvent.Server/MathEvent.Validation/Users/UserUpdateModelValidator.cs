﻿using MathEvent.Contracts.Validators;
using MathEvent.Models.Users;
using MathEvent.Models.Validation;
using MathEvent.Validation.Events;
using MathEvent.Validation.Organization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Validation.Users
{
    /// <summary>
    /// Валидатор модели обновления пользователя
    /// </summary>
    public class UserUpdateModelValidator : IUserUpdateModelValidator
    {
        private readonly UserValidationUtils _userValidationUtils;

        private readonly EventValidationUtils _eventValidationUtils;

        private readonly OrganizationValidationUtils _organizationValidationUtils;

        public UserUpdateModelValidator(
            UserValidationUtils userValidationUtils,
            EventValidationUtils eventValidationUtils,
            OrganizationValidationUtils organizationValidationUtils)
        {
            _userValidationUtils = userValidationUtils;
            _eventValidationUtils = eventValidationUtils;
            _organizationValidationUtils = organizationValidationUtils;
        }

        public async Task<ValidationResult> Validate(UserUpdateModel model)
        {
            var validationErrors = new List<ValidationError>();

            validationErrors.AddRange(await _userValidationUtils.ValidateIdentityUserId(model.IdentityUserId, false));
            validationErrors.AddRange(_userValidationUtils.ValidateName(model.Name));
            validationErrors.AddRange(_userValidationUtils.ValidateSurname(model.Surname));

            if (model.OrganizationId is not null)
            {
                validationErrors.AddRange(await _organizationValidationUtils.ValidateOrganizationId(model.OrganizationId));
            }

            if (model.Events is not null)
            {
                validationErrors.AddRange(await _eventValidationUtils.ValidateEventIds(model.Events));
            }

            if (model.ManagedEvents is not null)
            {
                validationErrors.AddRange(await _eventValidationUtils.ValidateEventIds(model.ManagedEvents));
            }

            return new ValidationResult
            {
                IsValid = validationErrors.Count < 1,
                Errors = validationErrors,
            };
        }
    }
}
