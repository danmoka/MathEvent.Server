using MathEvent.Contracts;
using MathEvent.Converters.Organizations.Models.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MathEvent.Converters.Identities.Models
{
    /// <summary>
    /// Класс для передачи данных для создания пользователя
    /// </summary>
    public class UserCreateModel : IValidatableObject
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

        [Required(ErrorMessage = "Пароль пользователя должен быть задан")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Длина пароля пользователя должна быть от 1 до 255 символов")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтверждение пароля пользователя должено быть задано")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Длина подтверждения пароля пользователя должна быть от 1 до 255 символов")]
        public string PasswordConfirm { get; set; }

        public int? OrganizationId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IRepositoryWrapper repositoryWrapper = (IRepositoryWrapper)validationContext.GetService(typeof(IRepositoryWrapper));

            yield return OrganizationValidationUtils.ValidateOrganizationId(OrganizationId, repositoryWrapper).Result;
        }
    }
}
