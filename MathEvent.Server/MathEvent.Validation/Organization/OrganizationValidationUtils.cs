using MathEvent.Contracts.Services;
using MathEvent.Contracts.Validators;
using MathEvent.Validation.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Validation.Organization
{
    /// <summary>
    /// Предназначен для описания нестандартной валидации данных, связанных с организацией
    /// </summary>
    public class OrganizationValidationUtils
    {
        private readonly IOrganizationService _organizationService;

        private const int _itnMaxLength = 12;

        private const int _itnMinLength = 10;

        private const int _nameMaxLength = 150;

        public OrganizationValidationUtils(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        /// <summary>
        /// Валидация ИНН
        /// </summary>
        /// <param name="itn">ИНН</param>
        /// <returns>Ошибки валидации</returns>
        public IEnumerable<IValidationError> ValidateITN(string itn)
        {
            var validationErrors = new List<IValidationError>();

            if (string.IsNullOrEmpty(itn))
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(itn),
                    Message = "ИНН не задано"
                });
            }
            else
            {
                if (itn.Length > _itnMaxLength)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(itn),
                        Message = $"Длина ИНН не должна превышать {_itnMaxLength} символов"
                    });
                }

                if (itn.Length < _itnMinLength)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(itn),
                        Message = $"Длина ИНН должна быть больше {_itnMinLength} символов"
                    });
                }
            }

            return validationErrors;
        }

        /// <summary>
        /// Валидация названия организации
        /// </summary>
        /// <param name="name">Название организации</param>
        /// <returns>Ошибки валидации</returns>
        public IEnumerable<IValidationError> ValidateName(string name)
        {
            var validationErrors = new List<IValidationError>();

            if (string.IsNullOrEmpty(name))
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(name),
                    Message = "Название не задано"
                });
            }
            else
            {
                if (name.Length > _nameMaxLength)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(name),
                        Message = $"Длина названия не должна превышать {_nameMaxLength} символов"
                    });
                }
            }

            return validationErrors;
        }

        /// <summary>
        /// Проверяет то, что существует ли организация с данным идентификатором
        /// </summary>
        /// <param name="id">id организации</param>
        /// <returns></returns>
        internal async Task<IEnumerable<IValidationError>> ValidateOrganizationId(int? id)
        {
            var validationErrors = new List<IValidationError>();

            if (id is null)
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(id),
                    Message = $"id={id} организации не задано"
                });
            }
            else
            {
                var organization = await _organizationService.RetrieveAsync((int)id);

                if (organization is null)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(id),
                        Message = $"Организации с id = {id} не существует"
                    });
                }
            }

            return validationErrors;
        }
    }
}
