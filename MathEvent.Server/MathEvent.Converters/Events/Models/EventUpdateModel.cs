using MathEvent.Converters.Events.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MathEvent.Converters.Events.Models
{
    /// <summary>
    /// Класс для передачи данных для обновления Event
    /// </summary>
    public class EventUpdateModel : IValidatableObject
    {
        [StringLength(250, MinimumLength = 1, ErrorMessage = "Длина названия события должна быть от 1 до 250 символов")]
        public string Name { get; set; }

        [StringLength(500, MinimumLength = 1, ErrorMessage = "Длина описания события должна быть от 1 до 500 символов")]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [StringLength(100, MinimumLength = 1, ErrorMessage = "Длина адреса события должна быть от 1 до 100 символов")]
        public string Location { get; set; }

        public int? OrganizationId { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        public int? ParentId { get; set; }
        #endregion

        /// <summary>
        /// Для обновления коллекции пользователей по их id
        /// </summary>
        public ICollection<string> ApplicationUsers { get; set; }

        public ICollection<string> Managers { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtils.ValidateDateTime(StartDate);
        }
    }
}
