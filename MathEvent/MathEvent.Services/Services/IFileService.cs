using MathEvent.Contracts;
using MathEvent.Converters.Files.Models;
using MathEvent.Entities.Entities;
using MathEvent.Services.Results;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MathEvent.Services.Services
{
    /// <summary>
    /// Декларирует функциональность сервисов файлов
    /// </summary>
    public interface IFileService : IService<
        FileReadModel,
        FileReadModel,
        FileCreateModel,
        FileUpdateModel,
        int,
        AResult<IMessage, File>>
    {
        Task<File> GetFileEntityAsync(int id);

        Task<AResult<IMessage, File>> Upload(IFormFile file, FileCreateModel fileCreateModel);

        AResult<IMessage, File> IsCorrectFile(IFormFile file);

        Task<System.IO.FileStream> Download(int id);
    }
}
