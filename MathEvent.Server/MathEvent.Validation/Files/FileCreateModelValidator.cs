using MathEvent.Contracts.Validators;
using MathEvent.Models.Files;
using MathEvent.Validation.Common;
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

        public async Task<IValidationResult> Validate(FileCreateModel model)
        {
            var validationErrors = new List<IValidationError>();

            validationErrors.AddRange(_fileValidationUtils.ValidateName(model.Name));

            if (model.OwnerId is null)
            {
                validationErrors.Add(new ValidationError
                {
                    Field = nameof(model.Name),
                    Message = "Неопределен владелец файла",
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
