using MathEvent.Entities.Models.Event.DTOs;
using System.Collections.Generic;

namespace MathEvent.Entities.Models.Identity.DTOs
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
        public ICollection<EventReadDTO> Events { get; set; }
    }
}
