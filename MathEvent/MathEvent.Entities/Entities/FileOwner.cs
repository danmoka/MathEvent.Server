using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathEvent.Entities.Entities
{
    /// <summary>
    /// Сущность владельца файла
    /// </summary>
    public class FileOwner
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Event")]
        public int? EventId { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
    }
}
