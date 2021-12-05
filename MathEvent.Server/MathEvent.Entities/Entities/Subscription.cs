using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathEvent.Entities.Entities
{
    /// <summary>
    /// Сущность подписки пользователя на событие
    /// </summary>
    public class Subscription
    {
        [ForeignKey("ApplicationUser")]
        public Guid ApplicationUserId;

        [ForeignKey("Event")]
        public int EventId;
    }
}
