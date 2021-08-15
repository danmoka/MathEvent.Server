using MathEvent.Contracts;
using MathEvent.Converters.Identities.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MathEvent.Converters.Organizations.Models
{
    /// <summary>
    /// Класс для передачи данных для обновления организации
    /// </summary>
    public class OrganizationUpdateModel
    {
        [StringLength(12, MinimumLength = 10)]
        public string ITN { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 1)]
        public string Name { get; set; }

        [Required]
        public string ManagerId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IRepositoryWrapper repositoryWrapper = (IRepositoryWrapper)validationContext.GetService(typeof(IRepositoryWrapper));

            yield return UserValidationUtils.ValidateUserId(ManagerId, repositoryWrapper).Result;
        }
    }
}
