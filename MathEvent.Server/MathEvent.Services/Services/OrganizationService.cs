using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Contracts.Services;
using MathEvent.DTOs.Organizations;
using MathEvent.Entities.Entities;
using MathEvent.Models.Organizations;
using MathEvent.Models.Others;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathEvent.Services.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IMapper _mapper;

        private readonly IRepositoryWrapper _repositoryWrapper;

        public OrganizationService(IMapper mapper, IRepositoryWrapper repositoryWrapper)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
        }

        /// <summary>
        /// Возвращает набор организаций
        /// </summary>
        /// <param name="filters">Набор пар ключ-значение</param>
        /// <returns>Набор организаций</returns>
        /// <remarks>Фильтры не используются</remarks>
        public async Task<IEnumerable<OrganizationReadModel>> List(IDictionary<string, string> filters)
        {
            var organizations = await Filter(_repositoryWrapper.Organization.FindAll(), filters);
            var organizationDTOs = _mapper.Map<IEnumerable<OrganizationDTO>>(organizations);
            var organizationReadModels = _mapper.Map<IEnumerable<OrganizationReadModel>>(organizationDTOs);

            return organizationReadModels;
        }

        /// <summary>
        /// Возвращает организацию с указанным id
        /// </summary>
        /// <param name="id">id организации</param>
        /// <returns>Организация с указанным id</returns>
        public async Task<OrganizationReadModel> Retrieve(int id)
        {
            var organization = await GetOrganizationEntityAsync(id);

            if (organization is null)
            {
                return null;
            }

            var organizationDTO = _mapper.Map<OrganizationDTO>(organization);
            var organizationReadModel = _mapper.Map<OrganizationReadModel>(organizationDTO);

            return organizationReadModel;
        }

        /// <summary>
        /// Создает организацию
        /// </summary>
        /// <param name="createModel">Модель создания организации</param>
        /// <returns>Созданная организация</returns>
        public async Task<OrganizationReadModel> Create(OrganizationCreateModel createModel)
        {
            var organizationEntity = _mapper.Map<Organization>(_mapper.Map<OrganizationDTO>(createModel));
            _repositoryWrapper.Organization.Create(organizationEntity);

            await _repositoryWrapper.SaveAsync();

            var organizationReadModel = _mapper.Map<OrganizationReadModel>(_mapper.Map<OrganizationDTO>(organizationEntity));

            return organizationReadModel;
        }

        /// <summary>
        /// Обновляет организацию
        /// </summary>
        /// <param name="id">id организации</param>
        /// <param name="updateModel">Модель обновления организации</param>
        /// <returns>Обновленная организация</returns>
        public async Task<OrganizationReadModel> Update(int id, OrganizationUpdateModel updateModel)
        {
            var organization = await GetOrganizationEntityAsync(id);

            if (organization is null)
            {
                throw new Exception($"Organization with id={id} not exists");
            }

            var organizationDTO = _mapper.Map<OrganizationDTO>(organization);
            _mapper.Map(updateModel, organizationDTO);
            _mapper.Map(organizationDTO, organization);

            _repositoryWrapper.Organization.Update(organization);
            await _repositoryWrapper.SaveAsync();

            var organizationReadModel = _mapper.Map<OrganizationReadModel>(_mapper.Map<OrganizationDTO>(organization));

            return organizationReadModel;
        }

        /// <summary>
        /// Удаляет организацию
        /// </summary>
        /// <param name="id">id организации</param>
        /// <returns></returns>
        public async Task Delete(int id)
        {
            var organization = await GetOrganizationEntityAsync(id);

            if (organization is null)
            {
                throw new Exception($"Organization with id={id} not exists");
            }

            _repositoryWrapper.Organization.Delete(organization);
            await _repositoryWrapper.SaveAsync();
        }

        /// <summary>
        /// Возвращает организацию с указанным id
        /// </summary>
        /// <param name="id">id организации</param>
        /// <returns>Организация</returns>
        private async Task<Organization> GetOrganizationEntityAsync(int id)
        {
            var organization = await _repositoryWrapper.Organization
                .FindByCondition(org => org.Id == id)
                .SingleOrDefaultAsync();

            return organization;
        }

        /// <summary>
        /// Возвращает набор статистик по организациям
        /// </summary>
        /// <param name="filters">Набор параметров в виде пар ключ-значение</param>
        /// <returns>Набор статистик по организациям</returns>
        /// <remarks>Фильтры не используются</remarks>
        public async Task<IEnumerable<ChartData>> GetOrganizationsStatistics(IDictionary<string, string> filters)
        {
            var statistics = new List<ChartData>();
            var mostPopularOrganizationStatistics = await GetMostPopularOrganizationStatistics();

            if (mostPopularOrganizationStatistics is not null)
            {
                statistics.Add(mostPopularOrganizationStatistics);
            }

            var mostProductiveOrganizationsStatistics = await GetMostProductiveOrganizationsStatistics();

            if (mostProductiveOrganizationsStatistics is not null)
            {
                statistics.Add(mostProductiveOrganizationsStatistics);
            }

            var countOfUsersInOrganizationStatistics = await GetCountOfUsersInOrganizationStatistics();

            if (countOfUsersInOrganizationStatistics is not null)
            {
                statistics.Add(countOfUsersInOrganizationStatistics);
            }

            return statistics;
        }

        private async Task<ChartData> GetMostProductiveOrganizationsStatistics()
        {
            var statistics = new ChartData
            {
                Title = $"Самые активные организации по количеству созданных событий за последний год",
                Type = ChartTypes.Bar,
                Data = new List<ChartDataPiece>()
            };

            var numberOfEventsPerMonthResult = await _repositoryWrapper.Event
                .FindByCondition(e => e.StartDate >= DateTime.UtcNow.AddYears(-1))
                .GroupBy(e => e.OrganizationId)
                .Select(g => new { orgId = g.Key, count = g.Count() })
                .ToDictionaryAsync(k => k.orgId is null ? -1 : k.orgId, i => i.count);

            foreach (var entry in numberOfEventsPerMonthResult)
            {
                var name = "Без организации";

                if (entry.Key != -1)
                {
                    var organization = await GetOrganizationEntityAsync(entry.Key.Value);

                    if (organization is null)
                    {
                        throw new Exception($"Organization with id={entry.Key.Value} not exists");
                    }

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

        private async Task<ChartData> GetMostPopularOrganizationStatistics()
        {
            var statistics = new ChartData
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
                .FindByCondition(e => e.StartDate >= DateTime.UtcNow.AddYears(-1))
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

                    if (organization is null)
                    {
                        throw new Exception($"Organization with id={entry.Key} not exists");
                    }

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

        private async Task<ChartData> GetCountOfUsersInOrganizationStatistics()
        {
            var statistics = new ChartData
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
                    var organization = await GetOrganizationEntityAsync(entry.Key.Value);

                    if (organization is null)
                    {
                        throw new Exception($"Organization with id={entry.Key.Value} not exists");
                    }

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

        private static async Task<IList<Organization>> Filter(IQueryable<Organization> organizationsQuery, IDictionary<string, string> filters)
        {
            var organizations = new List<Organization>();

            if (filters is not null)
            {
                organizations = await organizationsQuery.ToListAsync();

                if (filters.TryGetValue("search", out string searchString))
                {
                    if (!string.IsNullOrEmpty(searchString))
                    {
                        organizations = organizations.Where(c => c.Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                    }
                }
            }

            return organizations;
        }
    }
}
