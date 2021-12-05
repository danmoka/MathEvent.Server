using System.ComponentModel.DataAnnotations;

namespace MathEvent.Entities.Entities
{
    /// <summary>
    /// Сущность организации
    /// </summary>
    public class Organization
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 1)]
        public string Name { get; set; }

        [StringLength(12, MinimumLength = 10)]
        public string ITN { get; set; }

        [StringLength(350, MinimumLength = 1)]
        public string Description { get; set; }
    }
}
