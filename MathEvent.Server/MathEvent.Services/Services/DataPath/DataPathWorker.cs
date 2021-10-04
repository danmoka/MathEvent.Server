using MathEvent.Contracts.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MathEvent.Services.Services.DataPath
{
    /// <summary>
    /// Класс для работы с файлами и путями
    /// </summary>
    /// TODO: добавить ResultFactory
    public class DataPathWorker : IDataPathWorker
    {
        private readonly string _basePath;

        private readonly string _folder = "media"; // TODO: перенести в Configuration

        private readonly long _fileSizeLimit;

        private readonly FileExtensionWorker _fileExtensionManager;

        public DataPathWorker(string basePath, long fileSizeLimit, FileExtensionWorker fileExtensionManager)
        {
            _basePath = basePath;
            _fileSizeLimit = fileSizeLimit;
            _fileExtensionManager = fileExtensionManager;
        }

        /// <summary>
        /// Возвращает путь папки, в которую будет сохранен файл
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns></returns>
        private string GetPath(string userId)
        {
            var pathParts = new string[]
            {
                userId,
                DateTime.Now.Year.ToString(),
                DateTime.Now.Month.ToString(),
                DateTime.Now.Day.ToString()
            };

            return Path.Combine(pathParts);
        }

        /// <summary>
        /// Сохраняет файл
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Путь к файлу</returns>
        public async Task<string> Create(IFormFile file, string userId)
        {
            var path = GetPath(userId);
            var fullPath = GetFullPath(path);

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
        /// <param name="path">Путь</param>
        /// <returns>Поток файла</returns>
        public FileStream GetFileStream(string path)
        {
            var fullPath = GetFullPath(path);

            if (fullPath is null)
            {
                return null;
            }

            return File.OpenRead(fullPath);
        }

        /// <summary>
        /// Удаляет файл по указанному пути
        /// </summary>
        /// <param name="path">Путь</param>
        /// <param name="message">Сообщение о неудачном удалении</param>
        public void DeleteFile(string path, out string message)
        {
            message = null;
            var fullPath = GetFullPath(path);

            try
            {
                File.Delete(fullPath);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        /// <summary>
        /// Проверяет, является ли расширение файла разрешенным
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns>true, если расширение разрешено, false иначе</returns>
        public bool IsPermittedExtension(IFormFile file)
        {
            return _fileExtensionManager.IsCorrectFileExtensionAndSignature(file)
                || _fileExtensionManager.IsCorrectImgExtensionAndSignature(file);
        }

        /// <summary>
        /// Проверяет, является ли расширение изображения разрешенным
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns>true, если расширение разрешено, false иначе</returns>
        public bool IsPermittedImageExtension(IFormFile file)
        {
            return _fileExtensionManager.IsCorrectImgExtensionAndSignature(file);
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
        /// Возвращает полный путь
        /// </summary>
        /// <param name="path">Частичный путь</param>
        /// <returns>Полный путь</returns>
        private string GetFullPath(string path)
        {
            return Path.Combine(_basePath, _folder, path);
        }
    }
}
