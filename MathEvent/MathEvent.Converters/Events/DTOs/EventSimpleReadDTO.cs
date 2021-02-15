using System;

namespace MathEvent.Converters.Events.DTOs
{
    /// <summary>
    /// Класс для передачи упрощенной информации о событии
    /// </summary>
    public class EventSimpleReadDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }
    }
}
