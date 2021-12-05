using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MathEvent.Contracts.Services
{
    /// <summary>
    /// Определяет функциональность воркера с файлами и путями
    /// </summary>
    public interface IDataPathWorker
    {
        Task<string> CreateContentFile(IFormFile file, Guid userId);

        Task<string> CreateWebRootFile(IFormFile file, Guid userId);

        FileStream GetContentFileStream(string path);

        FileStream GetWebRootFileStream(string path);

        void DeleteContentFile(string path, out string message);

        void DeleteWebRootFile(string path, out string message);

        bool IsPermittedExtension(IFormFile file);

        bool IsPermittedImageExtension(IFormFile file);

        bool IsCorrectSize(IFormFile file);
    }
}
