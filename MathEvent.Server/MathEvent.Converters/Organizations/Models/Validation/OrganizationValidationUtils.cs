using MathEvent.Contracts;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MathEvent.Converters.Organizations.Models.Validation
{
    /// <summary>
    /// Предназначен для описания нестандартной валидации данных, связанных с организациями
    /// </summary>
    internal class OrganizationValidationUtils
    {
        /// <summary>
        /// Проверяет то, что существует ли организация с данным идентификатором
        /// </summary>
        /// <param name="id">id организации</param>
        /// <param name="repositoryWrapper">Обертка репозитория</param>
        /// <returns></returns>
        internal static async Task<ValidationResult> ValidateOrganizationId(int? id, IRepositoryWrapper repositoryWrapper)
        {
            if (id is not null)
            {
                var organization = await repositoryWrapper
                     .Organization
                     .FindByCondition(org => org.Id == id)
                     .SingleOrDefaultAsync();

                if (organization is null)
                {
                    return new ValidationResult(
                        $"Организации с id = {id} не существует",
                        new[] { nameof(id) });
                }
            }

            return ValidationResult.Success;
        }
    }
}
