using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Files.DTOs;
using MathEvent.Converters.Files.Models;
using MathEvent.Entities.Entities;
using MathEvent.Services.Messages;
using MathEvent.Services.Results;
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

        private readonly DataPathService _dataPathService;

        public FileService(IRepositoryWrapper repositoryWrapper, IMapper mapper, DataPathService dataPathService)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _dataPathService = dataPathService;
        }

        public async Task<IEnumerable<FileReadModel>> ListAsync(IDictionary<string, string> filters)
        {
            var files = await Filter(_repositoryWrapper.File.FindAll(), filters).ToListAsync();

            if (files is not null)
            {
                var filesDTO = _mapper.Map<IEnumerable<FileDTO>>(files);

                return _mapper.Map<IEnumerable<FileReadModel>>(filesDTO);
            }

            return null;
        }

        public async Task<FileReadModel> RetrieveAsync(int id)
        {
            var file = await GetFileEntityAsync(id);

            if (file is not null)
            {
                var fileDTO = _mapper.Map<FileDTO>(file);

                return _mapper.Map<FileReadModel>(fileDTO);
            }

            return null;
        }

        public async Task<AResult<IMessage, File>> CreateAsync(FileCreateModel createModel)
        {
            var fileDTO = _mapper.Map<FileDTO>(createModel);
            fileDTO.Date = DateTime.Now;
            var fileEntity = _mapper.Map<File>(fileDTO);

            if (fileEntity is null)
            {
                return new MessageResult<File>
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>
                {
                    new SimpleMessage
                    {
                        Message = "Errors when mapping model"
                    }
                }
                };
            }

            await _repositoryWrapper.File.CreateAsync(fileEntity);
            await _repositoryWrapper.SaveAsync();

            return new MessageResult<File>
            {
                Succeeded = true,
                Entity = fileEntity
            };
        }

        public async Task<AResult<IMessage, File>> UpdateAsync(int id, FileUpdateModel updateModel)
        {
            var fileEntity = await GetFileEntityAsync(id);

            if (fileEntity is null)
            {
                return new MessageResult<File>
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>
                    {
                        new SimpleMessage
                        {
                            Code = "404",
                            Message = $"File with the ID {id} not found"
                        }
                    }
                };
            }

            var fileDTO = _mapper.Map<FileDTO>(fileEntity);
            _mapper.Map(updateModel, fileDTO);
            _mapper.Map(fileDTO, fileEntity);
            _repositoryWrapper.File.Update(fileEntity);
            await _repositoryWrapper.SaveAsync();

            return new MessageResult<File>
            {
                Succeeded = true,
                Entity = fileEntity
            };
        }

        public async Task<AResult<IMessage, File>> DeleteAsync(int id)
        {
            var fileEntity = await GetFileEntityAsync(id);

            if (fileEntity is null)
            {
                return new MessageResult<File>
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>
                    {
                        new SimpleMessage
                        {
                            Code = "404",
                            Message = $"File with the ID {id} not found"
                        }
                    }
                };
            }

            if (fileEntity.Path is not null)
            {
                _dataPathService.DeleteFile(fileEntity.Path, out string deleteMessage);

                if (deleteMessage is not null)
                {
                    return new MessageResult<File>
                    {
                        Succeeded = false,
                        Messages = new List<SimpleMessage>
                        {
                            new SimpleMessage
                            {
                                Message = deleteMessage
                            }
                        }
                    };
                }
            }

            _repositoryWrapper.File.Delete(fileEntity);
            await _repositoryWrapper.SaveAsync();

            return new MessageResult<File>
            {
                Succeeded = true
            };
        }

        public async Task<AResult<IMessage, File>> Upload(IFormFile file, FileCreateModel fileCreateModel)
        {
            var fileResult = await _dataPathService.Create(file, fileCreateModel.AuthorId);

            if (!fileResult.Succeeded)
            {
                return new MessageResult<File>
                {
                    Succeeded = false,
                    Messages = fileResult.Messages
                };
            }

            var fileDTO = _mapper.Map<FileDTO>(fileCreateModel);

            if (fileDTO is null)
            {
                _dataPathService.DeleteFile(fileResult.Entity, out string deleteMessage);

                var messageResult = new MessageResult<File>
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>()
                    {
                        new SimpleMessage
                        {
                            Message = $"Errors when mapping model {fileCreateModel.Name}"
                        }
                    }
                };

                if (deleteMessage is not null)
                {
                    messageResult.Messages.Append(
                        new SimpleMessage
                        {
                            Message = deleteMessage
                        });
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

                var messageResult = new MessageResult<File>
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>()
                    {
                        new SimpleMessage
                        {
                            Message = $"Errors when mapping DTO {fileDTO.Name}"
                        }
                    }
                };

                if (deleteMessage is not null)
                {
                    messageResult.Messages.Append(
                        new SimpleMessage
                        {
                            Message = deleteMessage
                        });
                }

                return messageResult;
            }

            await _repositoryWrapper.File.CreateAsync(fileEntity);
            await _repositoryWrapper.SaveAsync();

            return new MessageResult<File>
            {
                Succeeded = true,
                Entity = fileEntity
            };
        }

        public async Task<System.IO.FileStream> Download(int id)
        {
            var file = await GetFileEntityAsync(id);

            if (file is null)
            {
                return null;
            }

            return _dataPathService.GetFileStream(file.Path);
        }

        public AResult<IMessage, File> IsCorrectFile(IFormFile file)
        {
            var messageResult = new MessageResult<File>
            {
                Succeeded = true,
                Messages = new List<SimpleMessage>()
            };

            if (!_dataPathService.IsPermittedExtension(file))
            {
                messageResult.Messages.Append(
                    new SimpleMessage
                    {
                        Message = $"Incorrect extension: {file.FileName}"
                    }); ;
            }

            if (!_dataPathService.IsCorrectSize(file))
            {
                messageResult.Messages.Append(
                    new SimpleMessage
                    {
                        Message = $"Wrong size: {file.FileName} ({file.Length})"
                    }); ;
            }

            return messageResult;
        }

        public async Task<File> GetFileEntityAsync(int id)
        {
            return await _repositoryWrapper.File
                .FindByCondition(f => f.Id == id)
                .SingleOrDefaultAsync();
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
                    else
                    {
                        filesQuery = filesQuery.Where(f => f.OwnerId == null);
                    }
                }
            }

            return filesQuery;
        }
    }
}
