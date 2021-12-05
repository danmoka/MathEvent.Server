using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathEvent.Entities.Entities
{
    /// <summary>
    /// Сущность менеджера события
    /// </summary>
    public class Management
    {
        [ForeignKey("ApplicationUser")]
        public Guid ApplicationUserId { get; set; }

        [ForeignKey("Events")]
        public int EventId { get; set; }
    }
}
