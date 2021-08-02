using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathEvent.Entities.Entities
{
    /// <summary>
    /// Сущность владельца
    /// </summary>
    public class Owner
    {
        public enum Type
        {
            File
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public Type OwnedType { get; set; }

        [ForeignKey("Event")]
        public int? EventId { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
    }
}
