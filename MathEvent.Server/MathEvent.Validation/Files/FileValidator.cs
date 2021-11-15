using MathEvent.Contracts.Services;
using MathEvent.Contracts.Validators;
using MathEvent.Validation.Common;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Validation.Files
{
    /// <summary>
    /// Валидатор файла
    /// </summary>
    public class FileValidator : IFileValidator
    {
        private readonly IDataPathWorker _dataPathWorker;

        public FileValidator(IDataPathWorker dataPathWorker)
        {
            _dataPathWorker = dataPathWorker;
        }

        public async Task<IValidationResult> Validate(IFormFile file)
        {
            var validationErrors = new List<IValidationError>();

            if (!_dataPathWorker.IsPermittedExtension(file))
            {
                validationErrors.Add(new ValidationError
                {
                    Field = "Extension",
                    Message = $"Недопустимое расширение файла: {file.FileName}"
                });
            }

            if (!_dataPathWorker.IsCorrectSize(file))
            {
                validationErrors.Add(new ValidationError
                {
                    Field = "Size",
                    Message = $"Недопустимый размер файла: {file.FileName} ({file.Length})"
                });
            }

            return await Task.FromResult(new ValidationResult
            {
                IsValid = validationErrors.Count < 1,
                Errors = validationErrors,
            });
        }
    }
}
