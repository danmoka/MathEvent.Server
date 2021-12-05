using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathEvent.Entities.Entities
{
    /// <summary>
    /// Сущность пользователя
    /// </summary>
    public class ApplicationUser
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid IdentityUserId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [ForeignKey("Organization")]
        public int? OrganizationId { get; set; }
    }
}