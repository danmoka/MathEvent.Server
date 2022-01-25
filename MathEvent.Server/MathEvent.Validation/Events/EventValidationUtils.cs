using MathEvent.Contracts.Services;
using MathEvent.Models.Validation;
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

        private const int _nameMaxLength = 250;

        private const int _descriptionMaxLength = 500;

        private const int _locationMaxLength = 100;

        public EventValidationUtils(IEventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// Проверяет время начала события
        /// </summary>
        /// <param name="startDateISOString">Время</param>
        /// <returns>Ошибки валидации</returns>
        public IEnumerable<ValidationError> ValidateStartDateTime(string startDateISOString)
        {
            var validationErrors = new List<ValidationError>();

            if (!DateTime.TryParse(startDateISOString, out DateTime dateTime))
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(startDateISOString),
                    Message = "Неподдерживаемый формат даты и времени",
                });
            }
            else if (dateTime.ToUniversalTime() < DateTime.UtcNow)
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(startDateISOString),
                    Message = "Дата не может быть меньше текущей",
                });
            }

            return validationErrors;
        }

        /// <summary>
        /// Валидация названия события
        /// </summary>
        /// <param name="name">Название события</param>
        /// <returns>Ошибки валидации</returns>
        public IEnumerable<ValidationError> ValidateName(string name)
        {
            var validationErrors = new List<ValidationError>();

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
                if (name.Length > _nameMaxLength)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(name),
                        Message = $"Длина названия события должна быть до {_nameMaxLength} символов",
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
        public IEnumerable<ValidationError> ValidateDescription(string description)
        {
            var validationErrors = new List<ValidationError>();

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
                if (description.Length > _descriptionMaxLength)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(description),
                        Message = $"Длина описания события должна быть от 1 до {_descriptionMaxLength} символов",
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
        public IEnumerable<ValidationError> ValidateLocation(string location)
        {
            var validationErrors = new List<ValidationError>();

            if (string.IsNullOrEmpty(location))
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(location),
                    Message = "Адрес события должен быть задан",
                });
            }
            else
            {
                if (location.Length > _nameMaxLength)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(location),
                        Message = $"Длина адреса события должна быть до {_locationMaxLength} символов",
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
        public async Task<IEnumerable<ValidationError>> ValidateParentEventId(int? id)
        {
            var validationErros = new List<ValidationError>();

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
                var ev = await _eventService.Retrieve((int)id);

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
        /// Проверяет иерархию события
        /// </summary>
        /// <param name="hierarchy">Тип</param>
        /// <param name="id">Идентификатор события</param>
        /// <returns>Ошибки валидации</returns>
        // TODO: добавить возможность изменять иерархию
        //public async Task<IEnumerable<IValidationError>> ValidateHierarchy(bool? hierarchy, int id)
        //{
        //    var validationErros = new List<IValidationError>();

        //    var events = await _eventService.GetChildEvents(id);

        //    if ((events as ICollection<EventReadModel>).Count > 0 && hierarchy is null)
        //    {
        //        validationErros.Add(new ValidationError
        //        {
        //            Field = nameof(id),
        //            Message = $"Событие с id = {id} имеет дочерние события",
        //        });
        //    }

        //    return validationErros;
        //}

        /// <summary>
        /// Проверяет то, что существуют ли события с данными идентификаторами
        /// </summary>
        /// <param name="eventIds">Набор идентификаторов</param>
        /// <returns>Ошибки валидации</returns>
        internal async Task<IEnumerable<ValidationError>> ValidateEventIds(IEnumerable<int> eventIds)
        {
            var validationErrors = new List<ValidationError>();

            foreach (var id in eventIds)
            {
                var ev = await _eventService.Retrieve(id);

                if (ev is null)
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
