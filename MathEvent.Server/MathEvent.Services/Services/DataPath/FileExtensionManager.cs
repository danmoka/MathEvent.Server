using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MathEvent.Services.Services.DataPath
{
    /// <summary>
    /// Класс для работы с расширениями файлов
    /// </summary>
    public class FileExtensionManager
    {
        private readonly IDictionary<string, List<byte[]>> _fileExtensionsAndSignatures = new Dictionary<string, List<byte[]>>
        {
            { ".jpeg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                }
            },
            { ".pdf", new List<byte[]>
                {
                    new byte[] { 0x25, 0x50, 0x44, 0x46 }
                }
            },
            { ".docx", new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                    new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 },
                }
            },
            { ".doc", new List<byte[]>
                {
                    new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 },
                    new byte[] { 0x0D, 0x44, 0x4F, 0x43 },
                    new byte[] { 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1, 0x00},
                    new byte[] { 0xDB, 0xA5, 0x2D, 0x00 },
                    new byte[] { 0xEC, 0xA5, 0xC1, 0x00 },
                }
            },
            { ".ppt", new List<byte[]>
                {
                    new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 },
                    new byte[] { 0x00, 0x6E, 0x1E, 0xF0 },
                    new byte[] { 0x0F, 0x00, 0xE8, 0x03},
                    new byte[] { 0xA0, 0x46, 0x1D, 0xF0 },
                    new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x0E, 0x00, 0x00, 0x00 },
                    new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x1C, 0x00, 0x00, 0x00 },
                    new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x43, 0x00, 0x00, 0x00 },
                }
            },
            { ".pptx", new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                    new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 },
                }
            },
            { ".xls", new List<byte[]>
                {
                    new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 },
                    new byte[] { 0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00 },
                    new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x10},
                    new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x1F },
                    new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x22 },
                    new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x23 },
                    new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x28 },
                    new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x29 },
                }
            },
            { ".xlsx", new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                    new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 },
                }
            },
            { ".zip", new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                    new byte[] { 0x50, 0x4B, 0x4C, 0x49, 0x54, 0x45 },
                    new byte[] { 0x50, 0x4B, 0x53, 0x70, 0x58},
                    new byte[] { 0x50, 0x4B, 0x05, 0x06 },
                    new byte[] { 0x50, 0x4B, 0x07, 0x08 },
                    new byte[] { 0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70 },
                    new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x01, 0x00 },
                }
            },
        };

        private readonly IDictionary<string, List<byte[]>> _imgExtensionsAndSignatures = new Dictionary<string, List<byte[]>>
        {
            { ".jpeg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                }
            },
            { ".jpg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                }
            },
            { ".png", new List<byte[]>
                {
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A },
                }
            },
            { ".bmp", new List<byte[]>
                {
                    new byte[] { 0x42, 0x4D },
                }
            },
        };

        /// <summary>
        /// Проверяет является ли расширение файла допустимым и корректным
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns>Результат проверки расширения</returns>
        public bool IsCorrectFileExtensionAndSignature(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);

            if (!string.IsNullOrEmpty(extension))
            {
                return IsCorrectExtensionAndSignature(extension, file.OpenReadStream(), _fileExtensionsAndSignatures);
            }

            return false;
        }

        /// <summary>
        /// Проверяет является ли расширение изображения допустимым и корректным
        /// </summary>
        /// <param name="img">Изображение</param>
        /// <returns>Результат проверки расширения</returns>
        public bool IsCorrectImgExtensionAndSignature(IFormFile img)
        {
            var extension = Path.GetExtension(img.FileName);

            if (!string.IsNullOrEmpty(extension))
            {
                return IsCorrectExtensionAndSignature(extension, img.OpenReadStream(), _imgExtensionsAndSignatures);
            }

            return false;
        }

        private static bool IsCorrectExtensionAndSignature(string extension, Stream stream, IDictionary<string, List<byte[]>> extenstionsAndSignatures)
        {
            using var reader = new BinaryReader(stream);

            if (extension == ".csv")
            {
                return true;
            }

            if (extenstionsAndSignatures.ContainsKey(extension))
            {
                var signatures = extenstionsAndSignatures[extension];
                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

                return signatures.Any(signature =>
                    headerBytes.Take(signature.Length).SequenceEqual(signature));
            }

            return false;
        }
    }
}
