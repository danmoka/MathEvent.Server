using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathEvent.Entities.Entities
{
    /// <summary>
    /// Сущность пользователя
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Surname { get; set; }

        [StringLength(50, MinimumLength = 1)]
        public string Patronymic { get; set; }

        [ForeignKey("Organization")]
        public int? OrganizationId { get; set; }
    }
}