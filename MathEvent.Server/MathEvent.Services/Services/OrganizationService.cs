using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Organizations.DTOs;
using MathEvent.Converters.Organizations.Models;
using MathEvent.Converters.Others;
using MathEvent.Entities.Entities;
using MathEvent.Services.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathEvent.Services.Services
{
    public class OrganizationService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        private readonly IMapper _mapper;

        public OrganizationService(IRepositoryWrapper repositoryWrapper, IMapper mapper)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
        }

        /// <summary>
        /// Возвращает результат с набором организаций
        /// </summary>
        /// <param name="filters">Набор пар ключ-значение</param>
        /// <returns>Результат с набором организаций</returns>
        /// <remarks>Фильтры не используются</remarks>
        public async Task<IResult<IMessage, IEnumerable<OrganizationReadModel>>> ListAsync(IDictionary<string, string> filters)
        {
            var organizations = await Filter(_repositoryWrapper.Organization.FindAll(), filters).ToListAsync();

            if (organizations is not null)
            {
                var organizaionsDTO = _mapper.Map<IEnumerable<OrganizationDTO>>(organizations);

                return ResultFactory.GetSuccessfulResult(_mapper.Map<IEnumerable<OrganizationReadModel>>(organizaionsDTO));
            }

            return ResultFactory.GetUnsuccessfulMessageResult<IEnumerable<OrganizationReadModel>>(new List<IMessage>()
            {
                MessageFactory.GetSimpleMessage("402", "The list of organizations is empty")
            });
        }

        /// <summary>
        /// Возвращает результат с организацией с указанным id
        /// </summary>
        /// <param name="id">id организации</param>
        /// <returns>Результат с организацией с указанным id</returns>
        public async Task<IResult<IMessage, OrganizationReadModel>> RetrieveAsync(int id)
        {
            var organizationResult = await GetOrganizationEntityAsync(id);

            if (!organizationResult.Succeeded)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<OrganizationReadModel>(organizationResult.Messages);
            }

            var organizationEntity = organizationResult.Entity;

            if (organizationEntity is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<OrganizationReadModel>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("404", $"Organization with id = {id} not found")
                });
            }

            var organizationDTO = _mapper.Map<OrganizationDTO>(organizationEntity);

            return ResultFactory.GetSuccessfulResult(_mapper.Map<OrganizationReadModel>(organizationDTO));
        }

        /// <summary>
        /// Создает организацию
        /// </summary>
        /// <param name="createModel">Модель создания организации</param>
        /// <returns>Результат создания организации</returns>
        public async Task<IResult<IMessage, Organization>> CreateAsync(OrganizationCreateModel createModel)
        {
            if (createModel.ManagerId is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<Organization>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("400", "Manager id is null")
                });
            }

            var organizationEntity = _mapper.Map<Organization>(_mapper.Map<OrganizationDTO>(createModel));

            if (organizationEntity is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<Organization>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage(null, $"Errors when mapping model {createModel.Name}")
                });
            }

            var organizationEntityDb = await _repositoryWrapper.Organization.CreateAsync(organizationEntity);

            if (organizationEntityDb is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<Organization>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage(null, $"Errors when creating entity {organizationEntity.Name}")
                });
            }

            await _repositoryWrapper.SaveAsync();

            return ResultFactory.GetSuccessfulResult(organizationEntityDb);
        }

        /// <summary>
        /// Обновляет организацию
        /// </summary>
        /// <param name="id">id организации</param>
        /// <param name="updateModel">Модель обновления организации</param>
        /// <returns>Результат обновления организации</returns>
        public async Task<IResult<IMessage, Organization>> UpdateAsync(int id, OrganizationUpdateModel updateModel)
        {
            if (updateModel.ManagerId is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<Organization>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("400", "Manager id is null")
                });
            }

            var organizationResult = await GetOrganizationEntityAsync(id);

            if (!organizationResult.Succeeded)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<Organization>(organizationResult.Messages);
            }

            var organizationEntity = organizationResult.Entity;

            if (organizationEntity is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<Organization>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("404", $"Organization with the ID {id} not found")
                });
            }

            var organizationDTO = _mapper.Map<OrganizationDTO>(organizationEntity);
            _mapper.Map(updateModel, organizationDTO);
            _mapper.Map(organizationDTO, organizationEntity);

            _repositoryWrapper.Organization.Update(organizationEntity);
            await _repositoryWrapper.SaveAsync();

            return ResultFactory.GetSuccessfulResult(organizationEntity);
        }

        /// <summary>
        /// Удаляет организацию
        /// </summary>
        /// <param name="id">id организации</param>
        /// <returns>Результат удаления организации</returns>
        public async Task<IResult<IMessage, Organization>> DeleteAsync(int id)
        {
            var organizationResult = await GetOrganizationEntityAsync(id);

            if (!organizationResult.Succeeded)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<Organization>(organizationResult.Messages);
            }

            var organizationEntity = organizationResult.Entity;

            if (organizationEntity is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<Organization>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("404", $"Organization with the ID {id} not found")
                });

            }

            _repositoryWrapper.Organization.Delete(organizationEntity);
            await _repositoryWrapper.SaveAsync();

            return ResultFactory.GetSuccessfulResult((Organization)null);
        }

        /// <summary>
        /// Возвращает результат с организацией с указанным id
        /// </summary>
        /// <param name="id">id организации</param>
        /// <returns>Результат с организацией с указанным id</returns>
        public async Task<IResult<IMessage, Organization>> GetOrganizationEntityAsync(int id)
        {
            var organization = await _repositoryWrapper.Organization
                .FindByCondition(org => org.Id == id)
                .SingleOrDefaultAsync();

            if (organization is null)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<Organization>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("404", $"Organization with id = {id} not found")
                });
            }

            return ResultFactory.GetSuccessfulResult(organization);
        }

        /// <summary>
        /// Возвращает результат с набором статистик по организациям
        /// </summary>
        /// <param name="filters">Набор параметров в виде пар ключ-значение</param>
        /// <returns>Результат с набором статистик по организациям</returns>
        /// <remarks>Фильтры не используются</remarks>
        public async Task<IResult<IMessage, IEnumerable<SimpleStatistics>>> GetSimpleStatistics(IDictionary<string, string> filters)
        {
            var simpleStatistics = new List<SimpleStatistics>();
            var mostPopularOrganizationStatistics = await GetMostPopularOrganizationStatistics();

            if (mostPopularOrganizationStatistics is not null)
            {
                simpleStatistics.Add(mostPopularOrganizationStatistics);
            }

            var mostProductiveOrganizationsStatistics = await GetMostProductiveOrganizationsStatistics();

            if (mostProductiveOrganizationsStatistics is not null)
            {
                simpleStatistics.Add(mostProductiveOrganizationsStatistics);
            }

            var countOfUsersInOrganizationStatistics = await GetCountOfUsersInOrganizationStatistics();

            if (countOfUsersInOrganizationStatistics is not null)
            {
                simpleStatistics.Add(countOfUsersInOrganizationStatistics);
            }

            if (simpleStatistics.Count < 1)
            {
                return ResultFactory.GetUnsuccessfulMessageResult<IEnumerable<SimpleStatistics>>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage("400", "Errors when getting a statistics for organizations")
                });
            }

            return ResultFactory.GetSuccessfulResult(simpleStatistics.AsEnumerable());
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
                    var organizationResult = await GetOrganizationEntityAsync(entry.Key.Value);

                    if (organizationResult.Succeeded)
                    {
                        var organization = organizationResult.Entity;

                        if (organization is not null)
                        {
                            name = organization.Name;
                        }
                    }
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
                    var organizationResult = await GetOrganizationEntityAsync(entry.Key);

                    if (organizationResult.Succeeded)
                    {
                        var organization = organizationResult.Entity;

                        if (organization is not null)
                        {
                            name = organization.Name;
                        }
                    }
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

        private async Task<SimpleStatistics> GetCountOfUsersInOrganizationStatistics()
        {
            var statistics = new SimpleStatistics
            {
                Title = $"Количество представителей организаций",
                Data = new List<ChartDataPiece>(),
                Type = ChartTypes.Bar
            };

            var usersCountPerOrganization = await _repositoryWrapper.User
                .FindAll()
                .GroupBy(u => u.OrganizationId)
                .Select(g => new { orgId = g.Key, count = g.Count() })
                .ToDictionaryAsync(k => k.orgId is null ? -1 : k.orgId, i => i.count);

            foreach (var entry in usersCountPerOrganization)
            {
                var name = "Без организации";

                if (entry.Key != -1)
                {
                    var organizationResult = await GetOrganizationEntityAsync(entry.Key.Value);

                    if (organizationResult.Succeeded)
                    {
                        var organization = organizationResult.Entity;

                        if (organization is not null)
                        {
                            name = organization.Name;
                        }
                    }
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
