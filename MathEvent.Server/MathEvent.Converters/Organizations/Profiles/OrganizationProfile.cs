using AutoMapper;
using MathEvent.DTOs.Organizations;
using MathEvent.Entities.Entities;
using MathEvent.Models.Organizations;

namespace MathEvent.Converters.Organizations.Profiles
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            //Source -> target

            // Model -> DTO
            CreateMap<OrganizationCreateModel, OrganizationDTO>();
            CreateMap<OrganizationUpdateModel, OrganizationDTO>();
            CreateMap<OrganizationReadModel, OrganizationDTO>();

            // DTO -> Model
            CreateMap<OrganizationDTO, OrganizationReadModel>(); // чтение
            CreateMap<OrganizationDTO, OrganizationUpdateModel>(); // обновление

            // DTO -> Entity
            CreateMap<OrganizationDTO, Organization>();

            // Entity -> DTO
            CreateMap<Organization, OrganizationDTO>();
        }
    }
}
