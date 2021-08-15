using MathEvent.Contracts;
using MathEvent.Converters.Events.Models.Validation;
using MathEvent.Converters.Identities.Models;
using MathEvent.Converters.Organizations.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MathEvent.Converters.Events.Models
{
    /// <summary>
    /// Класс для передачи данных для обновления события
    /// </summary>
    public class EventUpdateModel : IValidatableObject
    {
        [Required]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "Длина названия события должна быть от 1 до 250 символов")]
        public string Name { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Длина описания события должна быть от 1 до 500 символов")]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [MaxLength(100, ErrorMessage = "Длина адреса события должна быть до 100 символов")]
        public string Location { get; set; }

        public int? OrganizationId { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        public int? ParentId { get; set; }
        #endregion

        /// <summary>
        /// Для обновления коллекции пользователей по их id
        /// </summary>
        [Required]
        public ICollection<string> ApplicationUsers { get; set; }

        [Required]
        public ICollection<string> Managers { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IRepositoryWrapper repositoryWrapper = (IRepositoryWrapper)validationContext.GetService(typeof(IRepositoryWrapper));

            yield return EventValidationUtils.ValidateStartDateTime(StartDate);
            yield return OrganizationValidationUtils.ValidateOrganizationId(OrganizationId, repositoryWrapper).Result;
            yield return EventValidationUtils.ValidateParentEventId(ParentId, repositoryWrapper).Result;
            yield return UserValidationUtils.ValidateUserIds(ApplicationUsers, repositoryWrapper).Result;
            yield return UserValidationUtils.ValidateUserIds(Managers, repositoryWrapper).Result;
        }
    }
}
