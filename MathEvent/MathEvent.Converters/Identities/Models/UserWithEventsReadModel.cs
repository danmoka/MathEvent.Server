using MathEvent.Converters.Events.Models;
using System.Collections.Generic;

namespace MathEvent.Converters.Identities.Models
{
    /// <summary>
    /// Класс для передачи данных для чтения информации о пользователе и его событиях
    /// </summary>
    public class UserWithEventsReadModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Patronymic { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public ICollection<EventSimpleReadModel> Events { get; set; }

        public int? OwnerId { get; set; }
    }
}
