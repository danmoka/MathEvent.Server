using MathEvent.Contracts;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MathEvent.Converters.Files.Models.Validation
{
    /// <summary>
    /// Предназначен для описания нестандартной валидации данных, связанных с файлами
    /// </summary>
    internal static class FileValidationUtils
    {
        /// <summary>
        /// Проверяет то, что является ли файл родительским
        /// </summary>
        /// <param name="id">id файла</param>
        /// <param name="repositoryWrapper">Обертка репозитория</param>
        /// <returns>Результат валидации</returns>
        internal static async Task<ValidationResult> ValidateParentFileId(int? id, IRepositoryWrapper repositoryWrapper)
        {
            if (id is not null)
            {
                var file = await repositoryWrapper
                     .File
                     .FindByCondition(f => f.Id == id)
                     .SingleOrDefaultAsync();

                if (file is null)
                {
                    return new ValidationResult(
                        $"Файла с id = {id} не существует",
                        new[] { nameof(id) });
                }

                if (file.Hierarchy is null)
                {
                    return new ValidationResult(
                        $"Файл с id = {id} не папка",
                        new[] { nameof(id) });
                }
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Проверяет то, что существует ли файл с данным идентификатором
        /// </summary>
        /// <param name="id">id файла</param>
        /// <param name="repositoryWrapper">Обертка репозитория</param>
        /// <returns>Результат валидации</returns>
        internal static async Task<ValidationResult> ValidateFileId(int? id, IRepositoryWrapper repositoryWrapper)
        {
            if (id is not null)
            {
                var file = await repositoryWrapper
                     .File
                     .FindByCondition(f => f.Id == id)
                     .SingleOrDefaultAsync();

                if (file is null)
                {
                    return new ValidationResult(
                        $"Файла с id = {id} не существует",
                        new[] { nameof(id) });
                }
            }

            return ValidationResult.Success;
        }
    }
}
