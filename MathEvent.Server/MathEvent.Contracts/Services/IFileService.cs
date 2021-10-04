using MathEvent.Models.Files;
using MathEvent.Models.Others;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MathEvent.Contracts.Services
{
    /// <summary>
    /// Декларирует функциональность сервиса файлов
    /// </summary>
    public interface IFileService : IServiceBase<
        FileReadModel,
        FileReadModel,
        FileCreateModel,
        FileUpdateModel,
        int
        >
    {
        Task<FileReadModel> Upload(IFormFile file, FileCreateModel fileCreateModel);

        Task<FileStream> Download(int id);

        Task<IEnumerable<FileReadModel>> GetChildFiles(int id);

        Task<IEnumerable<Breadcrumb>> GetBreadcrumbs(int id);
    }
}
