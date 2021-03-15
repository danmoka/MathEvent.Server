using AutoMapper;
using MathEvent.Converters.Organizations.DTOs;
using MathEvent.Converters.Organizations.Models;
using MathEvent.Entities.Entities;

namespace MathEvent.Converters.Organizations.Profiles
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            //Source -> target

            // Model -> DTO
            CreateMap<OrganizaionCreateModel, OrganizationDTO>(); // создание
            CreateMap<OrganizationUpdateModel, OrganizationDTO>(); // обновление

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
