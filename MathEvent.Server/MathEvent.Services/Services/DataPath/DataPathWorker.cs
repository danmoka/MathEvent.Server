using MathEvent.Contracts.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MathEvent.Services.Services.DataPath
{
    /// <summary>
    /// Класс для работы с файлами и путями
    /// </summary>
    public class DataPathWorker : IDataPathWorker
    {
        private readonly string _baseWebRootPath;

        private readonly string _baseContentPath;

        private readonly string _webRootContentFolder;

        private readonly string _contentFolder;

        private readonly long _fileSizeLimit;

        private readonly IFileExtensionWorker _fileExtensionWorker;

        public DataPathWorker(
            IConfiguration configuration,
            IFileExtensionWorker fileExtensionWorker,
            IWebHostEnvironment webHostEnvironment)
        {
            var fileSettings = configuration.GetSection("Files");
            _baseWebRootPath = webHostEnvironment.WebRootPath;
            _baseContentPath = webHostEnvironment.ContentRootPath;
            _webRootContentFolder = fileSettings["WebRootContentFolder"];
            _contentFolder = fileSettings["ContentFolder"];
            _fileSizeLimit = long.Parse(fileSettings["FileSizeLimit"]);
            _fileExtensionWorker = fileExtensionWorker;
        }

        /// <summary>
        /// Возвращает путь папки, в которую будет сохранен файл
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns></returns>
        private string GetPath(Guid userId)
        {
            var pathParts = new string[]
            {
                userId.ToString(),
                DateTime.Now.Year.ToString(),
                DateTime.Now.Month.ToString(),
                DateTime.Now.Day.ToString()
            };

            return Path.Combine(pathParts);
        }

        /// <summary>
        /// Сохраняет файл контента
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Путь к файлу</returns>
        public async Task<string> CreateContentFile(IFormFile file, Guid userId)
        {
            var path = GetPath(userId);
            var fullPath = GetFullContentPath(path);

            return await CreateFile(file, path, fullPath);
        }

        /// <summary>
        /// Сохраняет публичный файл
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Путь к файлу</returns>
        public async Task<string> CreateWebRootFile(IFormFile file, Guid userId)
        {
            var path = GetPath(userId);
            var fullPath = GetFullWebRootPath(path);

            return await CreateFile(file, path, fullPath);
        }

        /// <summary>
        /// Предоставляет поток файла контента по указанному пути
        /// </summary>
        /// <param name="path">Путь</param>
        /// <returns>Поток файла</returns>
        public FileStream GetContentFileStream(string path)
        {
            var fullPath = GetFullContentPath(path);

            return GetFileStream(fullPath);
        }

        /// <summary>
        /// Предоставляет поток публичного файла по указанному пути
        /// </summary>
        /// <param name="path">Путь</param>
        /// <returns>Поток файла</returns>
        public FileStream GetWebRootFileStream(string path)
        {
            var fullPath = GetFullWebRootPath(path);

            return GetFileStream(fullPath);
        }

        /// <summary>
        /// Удаляет файл контента по указанному пути
        /// </summary>
        /// <param name="path">Путь</param>
        /// <param name="message">Сообщение о неудачном удалении</param>
        public void DeleteContentFile(string path, out string message)
        {
            var fullPath = GetFullContentPath(path);
            message = DeleteFile(fullPath);
        }

        /// <summary>
        /// Удаляет публичный файл по указанному пути
        /// </summary>
        /// <param name="path">Путь</param>
        /// <param name="message">Сообщение о неудачном удалении</param>
        public void DeleteWebRootFile(string path, out string message)
        {
            var fullPath = GetFullWebRootPath(path);
            message = DeleteFile(fullPath);
        }

        /// <summary>
        /// Проверяет, является ли расширение файла разрешенным
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns>true, если расширение разрешено, false иначе</returns>
        public bool IsPermittedExtension(IFormFile file)
        {
            return _fileExtensionWorker.IsCorrectFileExtensionAndSignature(file)
                || _fileExtensionWorker.IsCorrectImgExtensionAndSignature(file);
        }

        /// <summary>
        /// Проверяет, является ли расширение изображения разрешенным
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns>true, если расширение разрешено, false иначе</returns>
        public bool IsPermittedImageExtension(IFormFile file)
        {
            return _fileExtensionWorker.IsCorrectImgExtensionAndSignature(file);
        }

        /// <summary>
        /// Проверяет размер файла
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns>true, если размер корректен, false иначе</returns>
        public bool IsCorrectSize(IFormFile file)
        {
            return file.Length < _fileSizeLimit;
        }

        /// <summary>
        /// Сохраняет файл
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Путь к файлу</returns>
        private async Task<string> CreateFile(IFormFile file, string path, string fullPath)
        {
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            // рандомная часть названия + оригинальная с обрезанием пути (который может указать пользователь)
            var fileName = $"{Path.GetRandomFileName()}{Path.GetFileName(file.FileName)}";
            var fullFilePath = Path.Combine(fullPath, fileName);
            using var stream = new FileStream(fullFilePath, FileMode.Create);

            try
            {
                await file.CopyToAsync(stream);
            }
            finally
            {
                await stream.DisposeAsync();
            }

            return Path.Combine(path, fileName);
        }

        /// <summary>
        /// Предоставляет поток файла по указанному пути
        /// </summary>
        /// <param name="fullPath">Путь</param>
        /// <returns>Поток файла</returns>
        private FileStream GetFileStream(string fullPath)
        {
            if (fullPath is null)
            {
                return null;
            }

            return File.OpenRead(fullPath);
        }

        /// <summary>
        /// Удаляет файл по указанному пути
        /// </summary>
        /// <param name="fullPath">Путь</param>
        ///<returns>Сообщение о неудачном удалении</returns>
        private string DeleteFile(string fullPath)
        {
            try
            {
                File.Delete(fullPath);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        /// <summary>
        /// Возвращает полный путь до папки контента
        /// </summary>
        /// <param name="path">Частичный путь</param>
        /// <returns>Полный путь до папки контента</returns>
        private string GetFullContentPath(string path)
        {
            return Path.Combine(_baseContentPath, _contentFolder, path);
        }

        /// <summary>
        /// Возвращает полный путь до публичных файлов
        /// </summary>
        /// <param name="path">Частичный путь</param>
        /// <returns>Полный путь до публичных файлов</returns>
        private string GetFullWebRootPath(string path)
        {
            return Path.Combine(_baseWebRootPath, _webRootContentFolder, path);
        }
    }
}
