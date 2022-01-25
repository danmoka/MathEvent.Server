using MathEvent.Contracts.Validators;
using MathEvent.Models.Files;
using MathEvent.Models.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Validation.Files
{
    /// <summary>
    /// Валидатор модели создания файла
    /// </summary>
    public class FileCreateModelValidator : IFileCreateModelValidator
    {
        private readonly FileValidationUtils _fileValidationUtils;

        public FileCreateModelValidator(FileValidationUtils fileValidationUtils)
        {
            _fileValidationUtils = fileValidationUtils;
        }

        public async Task<ValidationResult> Validate(FileCreateModel model)
        {
            var validationErrors = new List<ValidationError>();

            validationErrors.AddRange(_fileValidationUtils.ValidateName(model.Name));

            if (model.OwnerId is null)
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(model.Name),
                    Message = "Не определен владелец файла",
                });
            }

            if (model.ParentId is not null)
            {
                var validationError = await _fileValidationUtils.ValidateParentFileId((int)model.ParentId);

                validationErrors.AddRange(validationError);
            }

            return new ValidationResult
            {
                IsValid = validationErrors.Count < 1,
                Errors = validationErrors,
            };
        }
    }
}
