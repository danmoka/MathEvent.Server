using MathEvent.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MathEvent.Converters.Identities.Models
{
    /// <summary>
    /// Предназначен для описания нестандартной валидации данных, связанных с пользователями
    /// </summary>
    internal static class UserValidationUtils
    {
        /// <summary>
        /// Проверяет то, что существуют ли пользователи с данными идентификаторами
        /// </summary>
        /// <param name="userIds">Набор идентификаторов</param>
        /// <param name="repositoryWrapper">Обертка репозитория</param>
        /// <returns>Результат валидации</returns>
        internal static async Task<ValidationResult> ValidateUserIds(IEnumerable<string> userIds, IRepositoryWrapper repositoryWrapper)
        {
            if (userIds is not null)
            {
                foreach (var id in userIds)
                {
                    var user = await repositoryWrapper
                        .User
                        .FindByCondition(u => u.Id == id)
                        .SingleOrDefaultAsync();

                    if (user is null)
                    {
                        return new ValidationResult(
                            $"Пользователя с id = {id} не существует",
                            new[] { nameof(id) });
                    }
                }
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Проверяет то, что существует ли пользователь с данным идентификатором
        /// </summary>
        /// <param name="id">id пользователя</param>
        /// <param name="repositoryWrapper">Обертка репозитория</param>
        /// <returns>Результат валидации</returns>
        internal static async Task<ValidationResult> ValidateUserId(string id, IRepositoryWrapper repositoryWrapper)
        {
            if (id is not null)
            {
                var user = await repositoryWrapper
                    .User
                    .FindByCondition(u => u.Id == id)
                    .SingleOrDefaultAsync();

                if (user is null)
                {
                    return new ValidationResult(
                        $"Пользователя с id = {id} не существует",
                        new[] { nameof(id) });
                }
            }

            return ValidationResult.Success;
        }
    }
}
