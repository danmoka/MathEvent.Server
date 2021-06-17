using MathEvent.Contracts;
using MathEvent.Services.Messages;
using MathEvent.Services.Results;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MathEvent.Services.Services
{
    /// <summary>
    /// Класс для работы с файлами и путями
    /// </summary>
    public class DataPathService
    {
        private readonly string _basePath;

        private readonly string _folder = "media";

        private readonly string[] _permittedExtensions = { ".pdf", ".docx", ".doc", ".ppt", ".pptx", ".png", ".jpg", ".jpeg", ".bmp" };

        private readonly string[] _permittedImageExtensions = { ".png", ".jpg", ".jpeg", ".bmp" };

        private readonly long _fileSizeLimit;

        public DataPathService(string basePath, long fileSizeLimit)
        {
            _basePath = basePath;
            _fileSizeLimit = fileSizeLimit;
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
        /// Возвращает полный путь
        /// </summary>
        /// <param name="path">Частичный путь</param>
        /// <returns>Полный путь</returns>
        private string GetFullPath(string path)
        {
            return Path.Combine(_basePath, _folder, path);
        }

        /// <summary>
        /// Сохраняет файл
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Результат создания файла</returns>
        public async Task<AResult<IMessage, string>> Create(IFormFile file, string userId)
        {
            var path = GetPath(userId);
            var fullPath = GetFullPath(path);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            var fileName = $"{Path.GetRandomFileName()}{file.FileName}";
            var fullFilePath = Path.Combine(fullPath, fileName);
            using var stream = new FileStream(fullFilePath, FileMode.Create);

            try
            {
                await file.CopyToAsync(stream);
            }
            catch (Exception ex)
            {
                return new MessageResult<string>()
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>()
                    {
                        new SimpleMessage
                        {
                            Message = ex.Message
                        }
                    }
                };
            }

            return new MessageResult<string>()
            {
                Succeeded = true,
                Entity = Path.Combine(path, fileName)
            };
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
            var extension = Path.GetExtension(file.FileName);

            if (!string.IsNullOrEmpty(extension) && _permittedExtensions.Contains(extension))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Проверяет, является ли расширение изображения разрешенным
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns>true, если расширение разрешено, false иначе</returns>
        public bool IsPermittedImageExtension(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);

            if (!string.IsNullOrEmpty(extension) && _permittedImageExtensions.Contains(extension))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Проверяет размер файла
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns>true, если размер корректен, false иначе</returns>
        public bool IsCorrectSize(IFormFile file)
        {
            if (file.Length < _fileSizeLimit)
            {
                return true;
            }

            return false;
        }
    }
}
