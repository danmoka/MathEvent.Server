using System;

namespace MathEvent.Entities.Models.Event.DTOs
{
    /// <summary>
    /// Класс для передачи данных для обновления Event
    /// </summary>
    public class EventUpdateDTO
    {
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        public int? ParentId { get; set; }
        #endregion

        public string Description { get; set; }
    }
}
