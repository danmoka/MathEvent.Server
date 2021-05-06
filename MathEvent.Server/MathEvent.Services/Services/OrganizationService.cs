using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Organizations.DTOs;
using MathEvent.Converters.Organizations.Models;
using MathEvent.Converters.Others;
using MathEvent.Entities.Entities;
using MathEvent.Services.Messages;
using MathEvent.Services.Results;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<IEnumerable<SimpleStatistics>> GetSimpleStatistics(IDictionary<string, string> filters)
        {
            ICollection<SimpleStatistics> simpleStatistics = new List<SimpleStatistics>
            {
                await GetMostPopularOrganizationStatistics(),
                await GetMostProductiveOrganizationsStatistics()
            };

            return simpleStatistics;
        }

        private async Task<SimpleStatistics> GetMostProductiveOrganizationsStatistics()
        {
            var statistics = new SimpleStatistics
            {
                Title = $"Самые активные организации по количеству созданных событий за последний год",
                Type = ChartTypes.Bar,
                Data = new List<ChartDataPiece>()
            };

            var numberOfEventsPerMonthResult = await _repositoryWrapper.Event
                .FindByCondition(e => e.StartDate >= DateTime.Now.AddYears(-1))
                .GroupBy(e => e.OrganizationId)
                .Select(g => new { orgId = g.Key, count = g.Count() })
                .ToDictionaryAsync(k => k.orgId is null ? -1 : k.orgId, i => i.count);

            foreach (var entry in numberOfEventsPerMonthResult)
            {
                var name = "Без организации";

                if (entry.Key != -1)
                {
                    var organization = await GetOrganizationEntityAsync(entry.Key.Value);
                    name = organization.Name;
                }

                statistics.Data.Add(
                    new ChartDataPiece
                    {
                        X = name,
                        Y = entry.Value
                    });
            }

            return statistics;
        }

        private async Task<SimpleStatistics> GetMostPopularOrganizationStatistics()
        {
            var statistics = new SimpleStatistics
            {
                Title = $"Топ самых популярных организаций по количеству подписчиков на их события за последний год",
                Data = new List<ChartDataPiece>()
            };

            var subscribersCountPerEvent = await _repositoryWrapper.Subscription
                .FindAll()
                .GroupBy(s => s.EventId)
                .Select(g => new { eventId = g.Key, count = g.Count() })
                .ToDictionaryAsync(k => k.eventId, i => i.count);

            var organizationSubcribersPerEvent = new Dictionary<int, int>();
            var events = await _repositoryWrapper.Event
                .FindByCondition(e => e.StartDate >= DateTime.Now.AddYears(-1))
                .ToListAsync();

            foreach (var ev in events)
            {
                if (subscribersCountPerEvent.ContainsKey(ev.Id))
                {
                    var subsCount = subscribersCountPerEvent[ev.Id];
                    var orgId = -1;

                    if (ev.OrganizationId is not null)
                    {
                        orgId = ev.OrganizationId.Value;
                    }

                    if (organizationSubcribersPerEvent.ContainsKey(orgId))
                    {
                        organizationSubcribersPerEvent[orgId] += subsCount;
                    }
                    else
                    {
                        organizationSubcribersPerEvent.Add(orgId, subsCount);
                    }
                }
            }

            foreach (var entry in organizationSubcribersPerEvent)
            {
                var name = "Без организации";

                if (entry.Key != -1)
                {
                    var organization = await GetOrganizationEntityAsync(entry.Key);
                    name = organization.Name;
                }

                statistics.Data.Add(
                    new ChartDataPiece
                    {
                        X = name,
                        Y = entry.Value
                    });
            }

            return statistics;
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
