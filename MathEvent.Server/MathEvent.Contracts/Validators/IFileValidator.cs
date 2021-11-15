using Microsoft.AspNetCore.Http;

namespace MathEvent.Contracts.Validators
{
    /// <summary>
    /// Декларирует функциональность валидатора файла
    /// </summary>
    public interface IFileValidator : IValidator<IFormFile>
    {
    }
}
