using MathEvent.Contracts.Services;
using MathEvent.Models.Validation;
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

        private const int _descriptionMaxLength = 350;

        public OrganizationValidationUtils(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        /// <summary>
        /// Валидация ИНН
        /// </summary>
        /// <param name="itn">ИНН</param>
        /// <returns>Ошибки валидации</returns>
        public async Task<IEnumerable<ValidationError>> ValidateITN(string itn, bool checkIfExists = false)
        {
            var validationErrors = new List<ValidationError>();

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

                if (checkIfExists && await _organizationService.FindByITN(itn) is not null)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(itn),
                        Message = $"Организация с ИНН = {itn} уже существует"
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
        public IEnumerable<ValidationError> ValidateName(string name)
        {
            var validationErrors = new List<ValidationError>();

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
        /// Валидация описания организации
        /// </summary>
        /// <param name="description">Описание организации</param>
        /// <returns>Ошибки валидации</returns>
        public IEnumerable<ValidationError> ValidateDescription(string description)
        {
            var validationErrors = new List<ValidationError>();

            if (string.IsNullOrEmpty(description))
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(description),
                    Message = "Описание не задано"
                });
            }
            else
            {
                if (description.Length > _descriptionMaxLength)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(description),
                        Message = $"Длина описания не должна превышать {_descriptionMaxLength} символов"
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
        internal async Task<IEnumerable<ValidationError>> ValidateOrganizationId(int? id)
        {
            var validationErrors = new List<ValidationError>();

            if (id is null)
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(id),
                    Message = $"id = {id} организации не задано"
                });
            }
            else
            {
                var organization = await _organizationService.Retrieve((int)id);

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
