using MathEvent.Converters.Events.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MathEvent.Converters.Events.Models
{
    /// <summary>
    /// Класс для передачи данных для создания события
    /// </summary>
    public class EventCreateModel : IValidatableObject
    {
        [Required(ErrorMessage = "Название события должно быть задано")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "Длина названия события должна быть от 1 до 250 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Описание события должно быть задано")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Длина описания события должна быть от 1 до 500 символов")]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [MaxLength(100, ErrorMessage = "Длина адреса события должна быть до 100 символов")]
        public string Location { get; set; }

        public string AuthorId { get; set; }

        public int? OrganizationId { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        public int? ParentId { get; set; }
        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtils.ValidateDateTime(StartDate);
        }
    }
}
