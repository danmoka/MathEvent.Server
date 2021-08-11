using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Identities.DTOs;
using MathEvent.Converters.Organizations.DTOs;
using MathEvent.Converters.Organizations.Models;
using MathEvent.Entities.Entities;
using System.Linq;

namespace MathEvent.Converters.Organizations.Profiles
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            //Source -> target

            // Model -> DTO
            CreateMap<OrganizationCreateModel, OrganizationDTO>()
                .ForMember(dest => dest.Manager, opt => opt.MapFrom<IdToManagerDTOResolverByOrganizationCreateModel>()); // создание
            CreateMap<OrganizationUpdateModel, OrganizationDTO>()
                .ForMember(dest => dest.Manager, opt => opt.MapFrom<IdToManagerDTOResolverByOrganizationUpdateModel>());// обновление

            // DTO -> Model
            CreateMap<OrganizationDTO, OrganizationReadModel>(); // чтение
            CreateMap<OrganizationDTO, OrganizationUpdateModel>(); // обновление

            // DTO -> Entity
            CreateMap<OrganizationDTO, Organization>();

            // Entity -> DTO
            CreateMap<Organization, OrganizationDTO>()
                .ForMember(dest => dest.Manager, opt => opt.MapFrom<IdToManagerDTOResolverByOrganization>());
        }

        private static UserDTO GetUserById(string id, IRepositoryWrapper repositoryWrapper, IMapper mapper)
        {
            return mapper.Map<UserDTO>(repositoryWrapper
                    .User
                    .FindByCondition(u => u.Id == id)
                    .SingleOrDefault());
        }

        /// <summary>
        /// Класс, описывающий маппинг id пользователя на transfer объект пользователя, связанного с управлением
        /// </summary>
        public class IdToManagerDTOResolverByOrganization : IValueResolver<Organization, OrganizationDTO, UserDTO>
        {
            private readonly IRepositoryWrapper _repositoryWrapper;
            private readonly IMapper _mapper;

            public IdToManagerDTOResolverByOrganization(IRepositoryWrapper repositoryWrapper, IMapper mapper)
            {
                _repositoryWrapper = repositoryWrapper;
                _mapper = mapper;
            }

            public UserDTO Resolve(Organization source, OrganizationDTO destination, UserDTO destMember, ResolutionContext context)
            {
                return GetUserById(source.ManagerId, _repositoryWrapper, _mapper);
            }
        }

        /// <summary>
        /// Класс, описывающий маппинг id пользователя на transfer объект пользователя, связанного с управлением
        /// </summary>
        public class IdToManagerDTOResolverByOrganizationCreateModel : IValueResolver<OrganizationCreateModel, OrganizationDTO, UserDTO>
        {
            private readonly IRepositoryWrapper _repositoryWrapper;
            private readonly IMapper _mapper;

            public IdToManagerDTOResolverByOrganizationCreateModel(IRepositoryWrapper repositoryWrapper, IMapper mapper)
            {
                _repositoryWrapper = repositoryWrapper;
                _mapper = mapper;
            }

            public UserDTO Resolve(OrganizationCreateModel source, OrganizationDTO destination, UserDTO destMember, ResolutionContext context)
            {
                return GetUserById(source.ManagerId, _repositoryWrapper, _mapper);
            }
        }

        /// <summary>
        /// Класс, описывающий маппинг id пользователя на transfer объект пользователя, связанного с управлением
        /// </summary>
        public class IdToManagerDTOResolverByOrganizationUpdateModel : IValueResolver<OrganizationUpdateModel, OrganizationDTO, UserDTO>
        {
            private readonly IRepositoryWrapper _repositoryWrapper;
            private readonly IMapper _mapper;

            public IdToManagerDTOResolverByOrganizationUpdateModel(IRepositoryWrapper repositoryWrapper, IMapper mapper)
            {
                _repositoryWrapper = repositoryWrapper;
                _mapper = mapper;
            }

            public UserDTO Resolve(OrganizationUpdateModel source, OrganizationDTO destination, UserDTO destMember, ResolutionContext context)
            {
                return GetUserById(source.ManagerId, _repositoryWrapper, _mapper);
            }
        }
    }
}
