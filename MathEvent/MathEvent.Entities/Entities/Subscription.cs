using System.ComponentModel.DataAnnotations.Schema;

namespace MathEvent.Entities.Entities
{
    /// <summary>
    /// Сущность подписки пользователя на событие
    /// </summary>
    public class Subscription
    {
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId;

        [ForeignKey("Event")]
        public int EventId;
    }
}
