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

        public string ITN { get; set; } // TODO: ограничение с помощью регулярки

        public string Name { get; set; }
    }
}
