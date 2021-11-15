using Microsoft.AspNetCore.Http;

namespace MathEvent.Contracts.Validators
{
    /// <summary>
    /// Декларирует функциональность валидатора изображения
    /// </summary>
    public interface IImageValidator : IValidator<IFormFile>
    {
    }
}
