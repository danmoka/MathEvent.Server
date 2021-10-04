using MathEvent.Contracts.Services;
using MathEvent.Contracts.Validators;
using MathEvent.Validation.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Validation.Events
{
    /// <summary>
    /// Предназначен для описания нестандартной валидации данных, связанных с событиями
    /// </summary>
    public class EventValidationUtils
    {
        private readonly IEventService _eventService;

        private const int _nameMaxLenght = 250;

        private const int _descriptionMaxLenght = 500;

        private const int _locationMaxLenght = 100;

        public EventValidationUtils(IEventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// Проверяет время начала события
        /// </summary>
        /// <param name="startDate">Время</param>
        /// <returns>Ошибки валидации</returns>
        public IEnumerable<IValidationError> ValidateStartDateTime(DateTime startDate)
        {
            var validationErrors = new List<IValidationError>();

            if (startDate.ToUniversalTime() < DateTime.UtcNow)
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(startDate),
                    Message = "Дата начала события не должна быть меньше текущей",
                });
            }

            return validationErrors;
        }

        /// <summary>
        /// Валидация названия события
        /// </summary>
        /// <param name="name">Название события</param>
        /// <returns>Ошибки валидации</returns>
        public IEnumerable<IValidationError> ValidateName(string name)
        {
            var validationErrors = new List<IValidationError>();

            if (string.IsNullOrEmpty(name))
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(name),
                    Message = "Название события должно быть задано",
                });
            }
            else
            {
                if (name.Length > _nameMaxLenght)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(name),
                        Message = $"Длина названия события должна быть до {_nameMaxLenght} символов",
                    });
                }
            }

            return validationErrors;
        }

        /// <summary>
        /// Валидация описания события
        /// </summary>
        /// <param name="description">Описание события</param>
        /// <returns>Ошибки валидации</returns>
        public IEnumerable<IValidationError> ValidateDescription(string description)
        {
            var validationErrors = new List<IValidationError>();

            if (string.IsNullOrEmpty(description))
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(description),
                    Message = "Описание события должно быть задано",
                });
            }
            else
            {
                if (description.Length > _descriptionMaxLenght)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(description),
                        Message = $"Длина описания события должна быть от 1 до {_descriptionMaxLenght} символов",
                    });
                }
            }

            return validationErrors;
        }

        /// <summary>
        /// Валидация адреса события
        /// </summary>
        /// <param name="location">Адрес</param>
        /// <returns>Ошибки валидации</returns>
        public IEnumerable<IValidationError> ValidateLocation(string location)
        {
            var validationErrors = new List<IValidationError>();

            if (string.IsNullOrEmpty(location))
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(location),
                    Message = "Адрес события должно быть задано",
                });
            }
            else
            {
                if (location.Length > _nameMaxLenght)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(location),
                        Message = $"Длина адреса события должна быть до {_locationMaxLenght} символов",
                    });
                }
            }

            return validationErrors;
        }

        /// <summary>
        /// Проверяет то, что является ли событие родительским
        /// </summary>
        /// <param name="id">id события</param>
        /// <returns>Ошибки валидации</returns>
        public async Task<IEnumerable<IValidationError>> ValidateParentEventId(int? id)
        {
            var validationErros = new List<IValidationError>();

            if (id is null)
            {
                validationErros.Add(new ValidationError
                {
                    Field = nameof(id),
                    Message = $"Идентификатор события не задан",
                });
            }
            else
            {
                var ev = await _eventService.RetrieveAsync((int)id);

                if (ev is null)
                {
                    validationErros.Add(new ValidationError
                    {
                        Field = nameof(id),
                        Message = $"События с id = {id} не существует",
                    });
                }
                else
                {
                    if (ev.Hierarchy is null)
                    {
                        validationErros.Add(new ValidationError
                        {
                            Field = nameof(id),
                            Message = $"Событие с id = {id} не может содержать другие события",
                        });
                    }
                }
            }

            return validationErros;
        }

        /// <summary>
        /// Проверяет то, что существуют ли события с данными идентификаторами
        /// </summary>
        /// <param name="eventIds">Набор идентификаторов</param>
        /// <returns>Ошибки валидации</returns>
        internal async Task<IEnumerable<IValidationError>> ValidateEventIds(IEnumerable<int> eventIds)
        {
            var validationErrors = new List<IValidationError>();

            foreach (var id in eventIds)
            {
                var user = await _eventService.RetrieveAsync(id);

                if (user is null)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(id),
                        Message = $"События с id = {id} не существует",
                    });
                }
            }

            return validationErrors;
        }
    }
}
