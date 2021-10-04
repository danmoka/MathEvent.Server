using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace MathEvent.Contracts.Services
{
    /// <summary>
    /// Определяет функциональность воркера с файлами и путями
    /// </summary>
    public interface IDataPathWorker
    {
        Task<string> Create(IFormFile file, string userId);

        FileStream GetFileStream(string path);

        void DeleteFile(string path, out string message);

        bool IsPermittedExtension(IFormFile file);

        bool IsPermittedImageExtension(IFormFile file);

        bool IsCorrectSize(IFormFile file);
    }
}
