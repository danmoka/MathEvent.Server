using MathEvent.Models.Organizations;
using MathEvent.Models.Others;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Contracts.Services
{
    /// <summary>
    /// Декларирует функциональность сервиса организаций
    /// </summary>
    public interface IOrganizationService : IServiceBase<
        OrganizationReadModel,
        OrganizationReadModel,
        OrganizationCreateModel,
        OrganizationUpdateModel,
        int>
    {
        Task<IEnumerable<ChartData>> GetOrganizationsStatistics(IDictionary<string, string> filters);
        Task<OrganizationReadModel> FindByITN(string itn);
    }
}
