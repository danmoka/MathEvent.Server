using MathEvent.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MathEvent.Converters.Events.Models.Validation
{
    /// <summary>
    /// Предназначен для описания нестандартной валидации данных, связанных с событиями
    /// </summary>
    internal static class EventValidationUtils
    {
        /// <summary>
        /// Проверяет время начала события
        /// </summary>
        /// <param name="time">Время</param>
        /// <returns>Результат валидации</returns>
        internal static ValidationResult ValidateStartDateTime(DateTime time)
        {
            if (time.ToUniversalTime() < DateTime.UtcNow)
            {
                return new ValidationResult(
                    "Дата начала события не должна быть меньше текущей",
                    new[] { nameof(time) });
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Проверяет то, что является ли событие родительским
        /// </summary>
        /// <param name="id">id события</param>
        /// <param name="repositoryWrapper">Обертка репозитория</param>
        /// <returns>Результат валидации</returns>
        internal static async Task<ValidationResult> ValidateParentEventId(int? id, IRepositoryWrapper repositoryWrapper)
        {
            if (id is not null)
            {
                var ev = await repositoryWrapper
                     .Event
                     .FindByCondition(e => e.Id == id)
                     .SingleOrDefaultAsync();

                if (ev is null)
                {
                    return new ValidationResult(
                        $"События с id = {id} не существует",
                        new[] { nameof(id) });
                }

                if (ev.Hierarchy is null)
                {
                    return new ValidationResult(
                        $"Событие с id = {id} не может содержать другие события",
                        new[] { nameof(id) });
                }
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Проверяет то, что существуют ли события с данными идентификаторами
        /// </summary>
        /// <param name="eventIds">Набор идентификаторов</param>
        /// <param name="repositoryWrapper">Обертка репозитория</param>
        /// <returns>Результат валидации</returns>
        internal static async Task<ValidationResult> ValidateEventIds(IEnumerable<int> eventIds, IRepositoryWrapper repositoryWrapper)
        {
            if (eventIds is not null)
            {
                foreach (var id in eventIds)
                {
                    var user = await repositoryWrapper
                        .Event
                        .FindByCondition(ev => ev.Id == id)
                        .SingleOrDefaultAsync();

                    if (user is null)
                    {
                        return new ValidationResult(
                            $"События с id = {id} не существует",
                            new[] { nameof(id) });
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
