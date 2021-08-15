using MathEvent.Contracts;
using MathEvent.Converters.Events.Models.Validation;
using MathEvent.Converters.Organizations.Models.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MathEvent.Converters.Identities.Models
{
    /// <summary>
    /// Класс для передачи данных для обновления пользователя
    /// </summary>
    public class UserUpdateModel : IValidatableObject
    {
        [Required(ErrorMessage = "Имя пользователя должно быть задано")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Длина имени пользователя должна быть от 1 до 50 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Фамилия пользователя должна быть задана")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Длина фамилии пользователя должна быть от 1 до 50 символов")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email пользователя должен быть задан")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Логин пользователя должен быть задан")]
        [StringLength(62, MinimumLength = 1, ErrorMessage = "Длина логина пользователя должна быть от 1 до 62 символов")]
        public string UserName { get; set; }

        [MaxLength(50, ErrorMessage = "Длина отчества пользователя должна быть до 50 символов")]
        public string Patronymic { get; set; }

        public int? OrganizationId { get; set; }

        public virtual ICollection<int> Events { get; set; }

        public virtual ICollection<int> ManagedEvents { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IRepositoryWrapper repositoryWrapper = (IRepositoryWrapper)validationContext.GetService(typeof(IRepositoryWrapper));

            yield return OrganizationValidationUtils.ValidateOrganizationId(OrganizationId, repositoryWrapper).Result;
            yield return EventValidationUtils.ValidateEventIds(Events, repositoryWrapper).Result;
            yield return EventValidationUtils.ValidateEventIds(ManagedEvents, repositoryWrapper).Result;
        }
    }
}
