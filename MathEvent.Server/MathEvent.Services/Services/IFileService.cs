using MathEvent.Contracts;
using MathEvent.Converters.Files.Models;
using MathEvent.Converters.Others;
using MathEvent.Entities.Entities;
using MathEvent.Services.Results;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Services.Services
{
    /// <summary>
    /// Декларирует функциональность сервисов файлов
    /// </summary>
    public interface IFileService : IControllerService<
        FileReadModel,
        FileReadModel,
        FileCreateModel,
        FileUpdateModel,
        int,
        AResult<IMessage, FileReadModel>>
    {
        Task<File> GetFileEntityAsync(int id);

        Task<AResult<IMessage, File>> Upload(IFormFile file, FileCreateModel fileCreateModel);

        AResult<IMessage, File> IsCorrectFile(IFormFile file);

        Task<System.IO.FileStream> Download(int id);

        Task<AResult<IMessage, IEnumerable<Breadcrumb>>> GetBreadcrumbs(int id);
    }
}
