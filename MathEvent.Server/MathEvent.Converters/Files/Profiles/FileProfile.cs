using AutoMapper;
using MathEvent.DTOs.Files;
using MathEvent.Entities.Entities;
using MathEvent.Models.Files;

namespace MathEvent.Converters.Files.Profiles
{
    /// <summary>
    /// Класс для маппинга моделей-transfer объектов-сущностей
    /// </summary>
    public class FileProfile : Profile
    {
        public FileProfile()
        {
            // Source -> target

            // Model -> DTO
            CreateMap<FileCreateModel, FileDTO>();
            CreateMap<FileUpdateModel, FileDTO>();

            // DTO -> Model
            CreateMap<FileDTO, FileReadModel>();
            CreateMap<FileDTO, FileUpdateModel>();

            // DTO -> Entity
            CreateMap<FileDTO, File>();

            // Entity -> DTO
            CreateMap<File, FileDTO>();
        }
    }
}
