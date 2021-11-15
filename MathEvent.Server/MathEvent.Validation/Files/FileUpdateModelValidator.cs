using MathEvent.Contracts.Validators;
using MathEvent.Models.Files;
using MathEvent.Validation.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Validation.Files
{
    /// <summary>
    /// Валидатор модели обновления файла
    /// </summary>
    public class FileUpdateModelValidator : IFileUpdateModelValidator
    {
        private readonly FileValidationUtils _fileValidationUtils;

        public FileUpdateModelValidator(FileValidationUtils fileValidationUtils)
        {
            _fileValidationUtils = fileValidationUtils;
        }

        public async Task<IValidationResult> Validate(FileUpdateModel model)
        {
            var validationErrors = new List<IValidationError>();

            validationErrors.AddRange(_fileValidationUtils.ValidateName(model.Name));

            return await Task.FromResult(new ValidationResult
            {
                IsValid = validationErrors.Count < 1,
                Errors = validationErrors,
            });
        }
    }
}
