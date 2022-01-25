using MathEvent.Contracts.Services;
using MathEvent.Contracts.Validators;
using MathEvent.Models.Validation;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Validation.Files
{
    /// <summary>
    /// Валидатор изображения
    /// </summary>
    public class ImageValidator : IImageValidator
    {
        private readonly IDataPathWorker _dataPathWorker;

        public ImageValidator(IDataPathWorker dataPathWorker)
        {
            _dataPathWorker = dataPathWorker;
        }

        public async Task<ValidationResult> Validate(IFormFile file)
        {
            var validationErrors = new List<ValidationError>();

            if (!_dataPathWorker.IsPermittedImageExtension(file))
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
