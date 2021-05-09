using System;

namespace MathEvent.Converters.Events.Models
{
    /// <summary>
    /// Класс для передачи упрощенной информации о событии
    /// </summary>
    public class EventSimpleReadModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public string AvatarPath { get; set; }
    }
}
