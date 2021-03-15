using MathEvent.Contracts;
using MathEvent.Converters.Organizations.Models;
using MathEvent.Entities.Entities;
using MathEvent.Services.Results;
using System.Threading.Tasks;

namespace MathEvent.Services.Services
{
    public interface IOrganizationService : IControllerService<
        OrganizationReadModel,
        OrganizationReadModel,
        OrganizaionCreateModel,
        OrganizationUpdateModel,
        int,
        AResult<IMessage, Organization>>
    {
        Task<Organization> GetOrganizationEntityAsync(int id);
    }
}
