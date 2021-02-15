using MathEvent.Converters.Events.DTOs;
using System.Collections.Generic;

namespace MathEvent.Converters.Identities.DTOs
{
    /// <summary>
    /// Класс для передачи данных для чтения информации о пользователе и его событиях
    /// </summary>
    public class UserWithEventsReadDTO
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Patronymic { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public ICollection<EventSimpleReadDTO> Events { get; set; }
    }
}
