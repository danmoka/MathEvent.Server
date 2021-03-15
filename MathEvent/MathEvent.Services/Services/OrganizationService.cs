using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Organizations.DTOs;
using MathEvent.Converters.Organizations.Models;
using MathEvent.Entities.Entities;
using MathEvent.Services.Messages;
using MathEvent.Services.Results;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathEvent.Services.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        private readonly IMapper _mapper;

        public OrganizationService(IRepositoryWrapper repositoryWrapper, IMapper mapper)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrganizationReadModel>> ListAsync(IDictionary<string, string> filters)
        {
            var organizations = await Filter(_repositoryWrapper.Organization.FindAll(), filters).ToListAsync();

            if (organizations is not null)
            {
                var organizaionsDTO = _mapper.Map<IEnumerable<OrganizationDTO>>(organizations);

                return _mapper.Map<IEnumerable<OrganizationReadModel>>(organizaionsDTO);
            }

            return null;
        }

        public async Task<OrganizationReadModel> RetrieveAsync(int id)
        {
            var organizationEntity = await GetOrganizationEntityAsync(id);

            if (organizationEntity is not null)
            {
                var organizationDTO = _mapper.Map<OrganizationDTO>(organizationEntity);

                return _mapper.Map<OrganizationReadModel>(organizationDTO);
            }

            return null;
        }

        public async Task<AResult<IMessage, Organization>> CreateAsync(OrganizaionCreateModel createModel)
        {
            var organizationEntity = _mapper.Map<Organization>(_mapper.Map<OrganizationDTO>(createModel));

            if (organizationEntity is null)
            {
                return new MessageResult<Organization>
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>()
                    {
                        new SimpleMessage
                        {
                            Message = $"Errors when mapping model {createModel.Name}"
                        }
                    }
                };
            }

            var organizationEntityDb = await _repositoryWrapper.Organization.CreateAsync(organizationEntity);

            if (organizationEntityDb is null)
            {
                return new MessageResult<Organization>
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>()
                    {
                        new SimpleMessage
                        {
                            Message = $"Errors when creating entity {organizationEntity.Name}"
                        }
                    }
                };
            }

            await _repositoryWrapper.SaveAsync();

            return new MessageResult<Organization>
            {
                Succeeded = true,
                Entity = organizationEntityDb
            };
        }

        public async Task<AResult<IMessage, Organization>> UpdateAsync(int id, OrganizationUpdateModel updateModel)
        {
            var organizationEntity = await GetOrganizationEntityAsync(id);

            if (organizationEntity is null)
            {
                return new MessageResult<Organization>
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>
                    {
                        new SimpleMessage
                        {
                            Code = "404",
                            Message = $"Organization with the ID {id} not found"
                        }
                    }
                };
            }

            var organizationDTO = _mapper.Map<OrganizationDTO>(organizationEntity);
            // TODO: добавить проверку после маппинга?
            _mapper.Map(updateModel, organizationDTO);
            _mapper.Map(organizationDTO, organizationEntity);
            _repositoryWrapper.Organization.Update(organizationEntity);
            await _repositoryWrapper.SaveAsync();

            return new MessageResult<Organization>
            {
                Succeeded = true,
                Entity = organizationEntity
            };
        }

        public async Task<AResult<IMessage, Organization>> DeleteAsync(int id)
        {
            var organizationEntity = await GetOrganizationEntityAsync(id);

            if (organizationEntity is null)
            {
                return new MessageResult<Organization>
                {
                    Succeeded = false,
                    Messages = new List<SimpleMessage>
                    {
                        new SimpleMessage
                        {
                            Code = "404",
                            Message = $"Organization with the ID {id} not found"
                        }
                    }
                };
            }

            _repositoryWrapper.Organization.Delete(organizationEntity);
            await _repositoryWrapper.SaveAsync();

            return new MessageResult<Organization> { Succeeded = true };
        }

        public async Task<Organization> GetOrganizationEntityAsync(int id)
        {
            return await _repositoryWrapper.Organization
                .FindByCondition(ev => ev.Id == id)
                .SingleOrDefaultAsync();
        }

        private static IQueryable<Organization> Filter(IQueryable<Organization> organizationQuery, IDictionary<string, string> filters)
        {
            if (filters is not null)
            {
                // TODO: фильтрация
            }

            return organizationQuery;
        }
    }
}
