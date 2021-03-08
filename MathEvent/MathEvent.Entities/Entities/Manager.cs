using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathEvent.Entities.Entities
{
    /// <summary>
    /// Сущность менеджера события
    /// </summary>
    public class Manager
    {
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }

        [ForeignKey("Events")]
        public int EventId { get; set; }
    }
}
