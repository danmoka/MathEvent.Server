using System.ComponentModel.DataAnnotations;

namespace MathEvent.Converters.Organizations.Models
{
    /// <summary>
    /// Класс для передачи данных для создания организации
    /// </summary>
    public class OrganizaionCreateModel
    {
        [StringLength(12, MinimumLength = 10)]
        public string ITN { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 1)]
        public string Name { get; set; }
    }
}
