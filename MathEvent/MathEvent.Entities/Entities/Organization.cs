using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathEvent.Entities.Entities
{
    /// <summary>
    /// Сущность организации
    /// </summary>
    [Table("Organizations")]
    public class Organization
    {
        [Key]
        public int Id { get; set; }

        public string ITN { get; set; }

        public string Name { get; set; }
    }
}
