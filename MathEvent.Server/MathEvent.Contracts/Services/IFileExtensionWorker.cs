using Microsoft.AspNetCore.Http;

namespace MathEvent.Contracts.Services
{
    /// <summary>
    /// Декларирует функциональность воркера с расширениями файлов
    /// </summary>
    public interface IFileExtensionWorker
    {
        bool IsCorrectFileExtensionAndSignature(IFormFile file);

        bool IsCorrectImgExtensionAndSignature(IFormFile img);
    }
}
