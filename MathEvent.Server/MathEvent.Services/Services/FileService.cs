using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Contracts.Services;
using MathEvent.DTOs.Files;
using MathEvent.Entities.Entities;
using MathEvent.Models.Files;
using MathEvent.Models.Others;
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
    public class FileService : IFileService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        private readonly IMapper _mapper;

        private readonly IDataPathWorker _dataPathWorker;

        private const uint _breadcrumbRecursionDepth = 8;

        public FileService(
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            IDataPathWorker dataPathWorker)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _dataPathWorker = dataPathWorker;
        }

        /// <summary>
        /// Предоставляет набор моделей файлов
        /// </summary>
        /// <param name="filters">Набор параметров - пар ключ-значение, для фильтрации файлов</param>
        /// <returns>Набор моделей файлов</returns>
        /// <remarks>Реализована фильтрация только по параметрам "parent" и "owner"</remarks>
        public async Task<IEnumerable<FileReadModel>> ListAsync(IDictionary<string, string> filters)
        {
            var files = await Filter(_repositoryWrapper.File.FindAll(), filters).ToListAsync();
            var fileDTOs = _mapper.Map<IEnumerable<FileDTO>>(files);
            var fileReadModels = _mapper.Map<IEnumerable<FileReadModel>>(fileDTOs);

            return fileReadModels;
        }

        /// <summary>
        /// Возвращает модель файл
        /// </summary>
        /// <param name="id">id файла</param>
        /// <returns>Модель файла</returns>
        public async Task<FileReadModel> RetrieveAsync(int id)
        {
            var file = await GetFileEntityAsync(id);

            if (file is null)
            {
                return null;
            }

            var fileDTO = _mapper.Map<FileDTO>(file);
            var fileReadModel = _mapper.Map<FileReadModel>(fileDTO);

            return fileReadModel;
        }

        /// <summary>
        /// Создает файл
        /// </summary>
        /// <param name="createModel">Модель создания файла</param>
        /// <returns>Модель созданного файла</returns>
        public async Task<FileReadModel> CreateAsync(FileCreateModel createModel)
        {
            var fileDTO = _mapper.Map<FileDTO>(createModel);
            fileDTO.Date = DateTime.UtcNow;
            var fileEntity = _mapper.Map<File>(fileDTO);

            var fileEntityDb = await _repositoryWrapper.File.CreateAsync(fileEntity);

            if (fileEntityDb is null)
            {
                throw new Exception("Errors while creating file");
            }

            await _repositoryWrapper.SaveAsync();

            var createdFileDTO = _mapper.Map<FileDTO>(fileEntityDb);
            var createdFileReadModel = _mapper.Map<FileReadModel>(createdFileDTO);

            return createdFileReadModel;
        }

        /// <summary>
        /// Обновляет файл
        /// </summary>
        /// <param name="id">id файла</param>
        /// <param name="updateModel">Модель обновления файла</param>
        /// <returns>Модель обновленного файла</returns>
        public async Task<FileReadModel> UpdateAsync(int id, FileUpdateModel updateModel)
        {
            var file = await GetFileEntityAsync(id);

            if (file is null)
            {
                throw new Exception($"File with id={id} is not exists");
            }

            var fileDTO = _mapper.Map<FileDTO>(file);
            _mapper.Map(updateModel, fileDTO);
            _mapper.Map(fileDTO, file);

            _repositoryWrapper.File.Update(file);
            await _repositoryWrapper.SaveAsync();

            var fileReadModel = _mapper.Map<FileReadModel>(fileDTO);

            return fileReadModel;
        }

        /// <summary>
        /// Удаляет файл
        /// </summary>
        /// <param name="id">id файла</param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var file = await GetFileEntityAsync(id);

            if (file is null)
            {
                throw new Exception($"File with id={id} is not exists");
            }

            if (file.Path is not null)
            {
                _dataPathWorker.DeleteContentFile(file.Path, out string deleteMessage);

                if (!string.IsNullOrEmpty(deleteMessage))
                {
                    throw new Exception(deleteMessage);
                }
            }

            _repositoryWrapper.File.Delete(file);
            await _repositoryWrapper.SaveAsync();
        }

        /// <summary>
        /// Загружает файл
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="fileCreateModel">Модель создания файла</param>
        /// <returns>Модель файла</returns>
        public async Task<FileReadModel> Upload(IFormFile file, FileCreateModel fileCreateModel)
        {
            var filePath = await _dataPathWorker.CreateContentFile(file, fileCreateModel.AuthorId);
            var fileDTO = _mapper.Map<FileDTO>(fileCreateModel);
            fileDTO.Extension = System.IO.Path.GetExtension(file.FileName);
            fileDTO.Path = filePath;
            fileDTO.Date = DateTime.UtcNow;

            var fileEntity = _mapper.Map<File>(fileDTO);

            var createdFile = await _repositoryWrapper.File.CreateAsync(fileEntity);

            if (createdFile is null)
            {
                throw new Exception("Errors while uploading file");
            }

            await _repositoryWrapper.SaveAsync();

            var fileReadModel = _mapper.Map<FileReadModel>(_mapper.Map<FileDTO>(createdFile));

            return fileReadModel;
        }

        /// <summary>
        /// Выгружает файл
        /// </summary>
        /// <param name="id">id файла</param>
        /// <returns>Поток файла</returns>
        public async Task<System.IO.FileStream> Download(int id)
        {
            var file = await GetFileEntityAsync(id);

            if (file is null)
            {
                throw new Exception($"File with id={id} is not exists");
            }

            var fileStream = _dataPathWorker.GetContentFileStream(file.Path);

            return fileStream;
        }

        /// <summary>
        /// Ищет дочерние файлы
        /// </summary>
        /// <param name="id">id файла</param>
        /// <returns>Дочерние файлы</returns>
        public async Task<IEnumerable<FileReadModel>> GetChildFiles(int id)
        {
            var childFiles = await _repositoryWrapper.File
                .FindByCondition(f => f.ParentId == id)
                .ToListAsync();
            var fileDTOs = _mapper.Map<IEnumerable<FileDTO>>(childFiles);
            var fileReadModels = _mapper.Map<IEnumerable<FileReadModel>>(fileDTOs);

            return fileReadModels;
        }

        /// <summary>
        /// Вовзращает набор-цепочку родительских файлов в виде хлебных крошек до файла с указанным id
        /// </summary>
        /// <param name="id">id файла, для которого требуется найти хлебные крошки</param>
        /// <returns>Набор-цепочка родительских файлов в виде хлебных крошек</returns>
        public async Task<IEnumerable<Breadcrumb>> GetBreadcrumbs(int id)
        {
            var file = await GetFileEntityAsync(id);

            if (file is null)
            {
                throw new Exception($"File with id={id} is not exists");
            }

            var breadcrumbs = new Stack<Breadcrumb>();

            breadcrumbs.Push(new Breadcrumb
            {
                Id = file.Id,
                Name = file.Name
            });

            var parent = await _repositoryWrapper.File
                .FindByCondition(e => e.Id == file.ParentId)
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

            return breadcrumbs;
        }

        /// <summary>
        /// Возвращает сущность файл с указанным id
        /// </summary>
        /// <param name="id">id файла</param>
        /// <returns>Сущность файла</returns>
        private async Task<File> GetFileEntityAsync(int id)
        {
            var file = await _repositoryWrapper.File
                .FindByCondition(f => f.Id == id)
                .SingleOrDefaultAsync();

            return file;
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
