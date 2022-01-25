using MathEvent.Contracts.Validators;
using MathEvent.Models.Files;
using MathEvent.Models.Validation;
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

        public async Task<ValidationResult> Validate(FileUpdateModel model)
        {
            var validationErrors = new List<ValidationError>();

            validationErrors.AddRange(_fileValidationUtils.ValidateName(model.Name));

            return await Task.FromResult(new ValidationResult
            {
                IsValid = validationErrors.Count < 1,
                Errors = validationErrors,
            });
        }
    }
}
