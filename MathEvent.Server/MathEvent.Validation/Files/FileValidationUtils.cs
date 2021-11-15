using MathEvent.Contracts.Services;
using MathEvent.Contracts.Validators;
using MathEvent.Validation.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Validation.Files
{
    /// <summary>
    /// Предназначен для описания нестандартной валидации данных, связанных с файлами
    /// </summary>
    public class FileValidationUtils
    {
        private readonly IFileService _fileService;

        private const int _nameMaxLenght = 250;

        public FileValidationUtils(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Проверяет то, что является ли файл родительским
        /// </summary>
        /// <param name="id">id файла</param>
        /// <returns>Ошибки валидации</returns>
        public async Task<IEnumerable<IValidationError>> ValidateParentFileId(int? id)
        {
            var validationErrors = new List<IValidationError>();

            if (id is null)
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(id),
                    Message = $"Идентификатор файла не задан",
                });
            }
            else
            {
                validationErrors.AddRange(await ValidateFileId(id));

                var file = await _fileService.RetrieveAsync((int)id);

                if (file.Hierarchy is null)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(id),
                        Message = $"Файл с id = {id} не папка",
                    });
                }
            }

            return validationErrors;
        }

        /// <summary>
        /// Проверяет то, что существует ли файл с данным идентификатором
        /// </summary>
        /// <param name="id">id файла</param>
        /// <returns>Ошибки валидации</returns>
        public async Task<IEnumerable<IValidationError>> ValidateFileId(int? id)
        {
            var validationErrors = new List<IValidationError>();

            if (id is null)
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(id),
                    Message = $"Идентификатор файла не задан",
                });
            }
            else
            {
                var file = await _fileService.RetrieveAsync((int)id);

                if (file is null)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(id),
                        Message = $"Файла с id = {id} не существует",
                    });
                }
            }

            return validationErrors;
        }

        /// <summary>
        /// Валидация названия файла
        /// </summary>
        /// <param name="name">Название файла</param>
        /// <returns>Ошибки валидации</returns>
        public IEnumerable<IValidationError> ValidateName(string name)
        {
            var validationErrors = new List<IValidationError>();

            if (string.IsNullOrEmpty(name))
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(name),
                    Message = "Название файла должно быть задано",
                });
            }
            else
            {
                if (name.Length > _nameMaxLenght)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Field = nameof(name),
                        Message = $"Длина названия файла должна быть до {_nameMaxLenght} символов",
                    });
                }
            }

            return validationErrors;
        }
    }
}
