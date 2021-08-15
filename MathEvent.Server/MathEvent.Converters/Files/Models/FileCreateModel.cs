using MathEvent.Contracts;
using MathEvent.Converters.Files.Models.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MathEvent.Converters.Files.Models
{
    /// <summary>
    /// Класс для передачи данных для создания файла
    /// </summary>
    public class FileCreateModel : IValidatableObject
    {
        [Required(ErrorMessage = "Имя должно быть задано")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "Длина названия должна быть от 1 до 250 символов")]
        public string Name { get; set; }

        public string AuthorId { get; set; }

        [Required(ErrorMessage = "Неопределен владелец файла")]
        public int? OwnerId { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        public int? ParentId { get; set; }
        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IRepositoryWrapper repositoryWrapper = (IRepositoryWrapper)validationContext.GetService(typeof(IRepositoryWrapper));

            yield return FileValidationUtils.ValidateParentFileId(ParentId, repositoryWrapper).Result;
        }
    }
}
