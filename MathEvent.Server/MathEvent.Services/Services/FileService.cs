using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Files.DTOs;
using MathEvent.Converters.Files.Models;
using MathEvent.Converters.Others;
using MathEvent.Entities.Entities;
using MathEvent.Services.Results;
using MathEvent.Services.Services.DataPath;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathEvent.Services.Services
{
    /// <summary>
    /// Сервис по выполнению действий над файлами
    /// </summary>
    public class FileService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        private readonly IMapper _mapper;

        private readonly DataPathService _dataPathService;

        private const uint _breadcrumbRecursionDepth = 8;

        public FileService(IRepositoryWrapper repositoryWrapper, IMapper mapper, DataPathService dataPathService)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _dataPathService = dataPathService;
        }

        /// <summary>
        /// Предоставляет результат с набором моделей на чтение файла
        /// </summary>
        /// <param name="filters">Набор параметров - пар ключ-значение, которым должны соответствовать файлы</param>
        /// <returns>Результат с набором моделей на чтение файла</returns>
        /// <remarks>Реализована фильтрация только по параметрам "parent" и "owner"</remarks>
        public async Task<IResult<IMessage, IEnumerable<FileReadModel>>> ListAsync(IDictionary<string, string> filters)
        {
            var files = await Filter(_repositoryWrapper.File.FindAll(), filters).ToListAsync();

            if (files is not null)
            {
                var filesDTO = _mapper.Map<IEnumerable<FileDTO>>(files);

                return ResultFactory.GetSuccessfulResult(_mapper.Map<IEnumerable<FileReadModel>>(filesDTO));
            }

            return ResultFactory.GetUnsuccessfulMessageResult<IEnumerable<FileReadModel>>(new List<IMessage>()
            {
                MessageFactory.GetSimpleMessage("402", "The list of files is empty")
            });
        }

        /// <summary>
        /// Возвращает результат с моделью на чтение файла
        /// </summary>
        /// <param name="id">id файла</param>
        /// <returns>Результат с моделью на чтение файла</returns>
        public async Task<IResult<IMessage, FileReadModel>> RetrieveAsync(int id)
        {
            var fileResult = await GetFileEntityAsync(id);

            if (!fileResult.Succeeded)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<FileReadModel>(fileResult.Messages);
            }

            var file = fileResult.Entity;

            if (file is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<FileReadModel>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("404", $"File with the ID {id} not found")
                });
            }

            return ResultFactory.GetSuccessfulResult(_mapper.Map<FileReadModel>(_mapper.Map<FileDTO>(file)));
        }

        /// <summary>
        /// Создает файл
        /// </summary>
        /// <param name="createModel">Модель создания файла</param>
        /// <returns>Результат создания файла</returns>
        public async Task<IResult<IMessage, FileReadModel>> CreateAsync(FileCreateModel createModel)
        {
            var fileDTO = _mapper.Map<FileDTO>(createModel);
            fileDTO.Date = DateTime.Now;
            var fileEntity = _mapper.Map<File>(fileDTO);

            if (fileEntity is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<FileReadModel>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage(null, "Errors when mapping model")
                });
            }

            await _repositoryWrapper.File.CreateAsync(fileEntity);
            await _repositoryWrapper.SaveAsync();

            return ResultFactory.GetSuccessfulResult(_mapper.Map<FileReadModel>(_mapper.Map<FileDTO>(fileEntity)));
        }

        /// <summary>
        /// Обновляет файл
        /// </summary>
        /// <param name="id">id файла</param>
        /// <param name="updateModel">Модель обновления файла</param>
        /// <returns>Результат обновления файла</returns>
        public async Task<IResult<IMessage, FileReadModel>> UpdateAsync(int id, FileUpdateModel updateModel)
        {
            var fileResult = await GetFileEntityAsync(id);

            if (!fileResult.Succeeded)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<FileReadModel>(fileResult.Messages);
            }

            var fileEntity = fileResult.Entity;

            if (fileEntity is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<FileReadModel>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("404", $"File with the ID {id} not found")
                });
            }

            var fileDTO = _mapper.Map<FileDTO>(fileEntity);
            _mapper.Map(updateModel, fileDTO);
            _mapper.Map(fileDTO, fileEntity);

            _repositoryWrapper.File.Update(fileEntity);
            await _repositoryWrapper.SaveAsync();

            return ResultFactory.GetSuccessfulResult(_mapper.Map<FileReadModel>(_mapper.Map<FileDTO>(fileEntity)));
        }

        /// <summary>
        /// Удаляет файл
        /// </summary>
        /// <param name="id">id файла</param>
        /// <returns>Результат удаления файла</returns>
        public async Task<IResult<IMessage, FileReadModel>> DeleteAsync(int id)
        {
            var fileResult = await GetFileEntityAsync(id);

            if (!fileResult.Succeeded)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<FileReadModel>(fileResult.Messages);
            }

            var fileEntity = fileResult.Entity;

            if (fileEntity is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<FileReadModel>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("404", $"File with the ID {id} not found")
                });
            }

            if (fileEntity.Path is not null)
            {
                _dataPathService.DeleteFile(fileEntity.Path, out string deleteMessage);

                if (deleteMessage is not null)
                {
                    return ResultFactory.GetUnsuccessfulMessageResult<FileReadModel>(new List<IMessage>()
                    {
                        MessageFactory.GetSimpleMessage(null, deleteMessage)
                    });
                }
            }

            _repositoryWrapper.File.Delete(fileEntity);
            await _repositoryWrapper.SaveAsync();

            return ResultFactory.GetSuccessfulResult((FileReadModel)null);
        }

        /// <summary>
        /// Загружает файл
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="fileCreateModel">Модель создания файла</param>
        /// <returns>Результат загрузки файла</returns>
        public async Task<IResult<IMessage, File>> Upload(IFormFile file, FileCreateModel fileCreateModel)
        {
            var fileResult = await _dataPathService.Create(file, fileCreateModel.AuthorId);

            if (!fileResult.Succeeded)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<File>(fileResult.Messages);
            }

            var fileDTO = _mapper.Map<FileDTO>(fileCreateModel);

            if (fileDTO is null)
            {
                _dataPathService.DeleteFile(fileResult.Entity, out string deleteMessage);

                var messageResult = ResultFactory.GetUnsuccessfulMessageResult<File>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage(null, $"Errors when mapping model {fileCreateModel.Name}")
                });

                if (deleteMessage is not null)
                {
                    messageResult.Messages = messageResult.Messages.Append(MessageFactory.GetSimpleMessage(null, deleteMessage));
                }

                return messageResult;
            }

            fileDTO.Extension = System.IO.Path.GetExtension(file.FileName);
            fileDTO.Path = fileResult.Entity;
            fileDTO.Date = DateTime.Now;

            var fileEntity = _mapper.Map<File>(fileDTO);

            if (fileEntity is null)
            {
                _dataPathService.DeleteFile(fileDTO.Path, out string deleteMessage);
                var messageResult = ResultFactory.GetUnsuccessfulMessageResult<File>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage(null, $"Errors when mapping DTO {fileDTO.Name}")
                });

                if (deleteMessage is not null)
                {
                    messageResult.Messages = messageResult.Messages.Append(MessageFactory.GetSimpleMessage(null, deleteMessage));
                }

                return messageResult;
            }

            await _repositoryWrapper.File.CreateAsync(fileEntity);
            await _repositoryWrapper.SaveAsync();

            return ResultFactory.GetSuccessfulResult(fileEntity);
        }

        /// <summary>
        /// Выгружает файл
        /// </summary>
        /// <param name="id">id файла</param>
        /// <returns>Результат с потоком файла</returns>
        public async Task<IResult<IMessage, System.IO.FileStream>> Download(int id)
        {
            var fileResult = await GetFileEntityAsync(id);

            if (!fileResult.Succeeded)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<System.IO.FileStream>(fileResult.Messages);
            }

            var file = fileResult.Entity;

            if (file is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<System.IO.FileStream>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("404", $"File with the ID {id} not found")
                });
            }

            return ResultFactory.GetSuccessfulResult(_dataPathService.GetFileStream(file.Path));
        }

        /// <summary>
        /// Проверяет файл на корректность
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns>Результат проверки</returns>
        public IResult<IMessage, File> IsCorrectFile(IFormFile file)
        {
            var messages = new List<IMessage>();

            if (!_dataPathService.IsPermittedExtension(file))
            {
                messages.Add(MessageFactory.GetSimpleMessage(null, $"Incorrect extension: {file.FileName}"));
            }

            if (!_dataPathService.IsCorrectSize(file))
            {
                messages.Add(MessageFactory.GetSimpleMessage(null, $"Wrong size: {file.FileName} ({file.Length})"));
            }

            if (messages.Count < 1)
            {
                return ResultFactory.GetSuccessfulResult((File)null);
            }
            else
            {
                return ResultFactory.GetUnsuccessfulMessageResult<File>(messages);
            }
        }

        /// <summary>
        /// Проверяет файл изображения на корректность
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns>Результат проверки</returns>
        public IResult<IMessage, File> IsCorrectImage(IFormFile file)
        {
            var messages = new List<IMessage>();

            if (!_dataPathService.IsPermittedImageExtension(file))
            {
                messages.Add(MessageFactory.GetSimpleMessage(null, $"Incorrect extension: {file.FileName}"));
            }

            if (!_dataPathService.IsCorrectSize(file))
            {
                messages.Add(MessageFactory.GetSimpleMessage(null, $"Wrong size: {file.FileName} ({file.Length})"));
            }

            if (messages.Count < 1)
            {
                return ResultFactory.GetSuccessfulResult((File)null);
            }
            else
            {
                return ResultFactory.GetUnsuccessfulMessageResult<File>(messages);
            }
        }

        /// <summary>
        /// Возвращает результат с файлом с указанным id
        /// </summary>
        /// <param name="id">id файла</param>
        /// <returns>Результат с файлом</returns>
        public async Task<IResult<IMessage, File>> GetFileEntityAsync(int id)
        {
            var file = await _repositoryWrapper.File
                .FindByCondition(f => f.Id == id)
                .SingleOrDefaultAsync();

            if (file is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<File>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("404", $"File with the ID {id} not found")
                });
            }

            return ResultFactory.GetSuccessfulResult(file);
        }

        /// <summary>
        /// Ищет дочерние файлы
        /// </summary>
        /// <param name="id">id файла</param>
        /// <returns>Результат поиска. Если файл не имеет дочерних файлов, то возвращается результат неуспеха</returns>
        public async Task<IResult<IMessage, IEnumerable<File>>> GetChildFiles(int id)
        {
            var childFiles = await _repositoryWrapper.File
                .FindByCondition(f => f.ParentId == id)
                .ToListAsync();

            if (childFiles.Count < 1)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<IEnumerable<File>>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage(null, $"File with id = {id} has no child files")
                });
            }

            return ResultFactory.GetSuccessfulMessageResult(
                new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage(null, $"File with id = {id} has child files")
                },
                childFiles.AsEnumerable());
        }

        /// <summary>
        /// Вовзращает набор-цепочку родительских файлов в виде хлебных крошек до файла с указанным id
        /// </summary>
        /// <param name="id">id файла, для которого требуется найти хлебные крошки</param>
        /// <returns>Набор-цепочка родительских файлов в виде хлебных крошек</returns>
        public async Task<IResult<IMessage, IEnumerable<Breadcrumb>>> GetBreadcrumbs(int id)
        {
            var breadcrumbs = new Stack<Breadcrumb>();
            var fileResult = await GetFileEntityAsync(id);

            if (!fileResult.Succeeded)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<IEnumerable<Breadcrumb>>(fileResult.Messages);
            }

            var currentFile = fileResult.Entity;

            if (currentFile is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<IEnumerable<Breadcrumb>>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("404", $"File with the ID {id} not found")
                });
            }

            breadcrumbs.Push(new Breadcrumb
            {
                Id = currentFile.Id,
                Name = currentFile.Name
            });

            var parent = await _repositoryWrapper.File
                .FindByCondition(e => e.Id == currentFile.ParentId)
                .SingleOrDefaultAsync();
            var depth = _breadcrumbRecursionDepth;

            while (parent != null && depth > 0)
            {
                breadcrumbs.Push(new Breadcrumb
                {
                    Id = parent.Id,
                    Name = parent.Name
                });
                parent = await _repositoryWrapper.File
                    .FindByCondition(e => e.Id == parent.ParentId)
                    .SingleOrDefaultAsync();
                depth--;
            }

            return ResultFactory.GetSuccessfulResult(breadcrumbs.AsEnumerable());
        }

        private static IQueryable<File> Filter(IQueryable<File> filesQuery, IDictionary<string, string> filters)
        {
            if (filters is not null)
            {
                if (filters.TryGetValue("parent", out string parentParam))
                {
                    if (int.TryParse(parentParam, out int parentId))
                    {
                        filesQuery = filesQuery.Where(f => f.ParentId == parentId);
                    }
                    else
                    {
                        filesQuery = filesQuery.Where(f => f.ParentId == null);
                    }
                }

                if (filters.TryGetValue("owner", out string ownerParam))
                {
                    if (int.TryParse(ownerParam, out int ownerId))
                    {
                        filesQuery = filesQuery.Where(f => f.OwnerId == ownerId);
                    }
                }
            }

            return filesQuery;
        }
    }
}
