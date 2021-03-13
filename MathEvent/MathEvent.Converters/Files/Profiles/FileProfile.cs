using AutoMapper;
using MathEvent.Converters.Files.DTOs;
using MathEvent.Converters.Files.Models;
using MathEvent.Entities.Entities;

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
