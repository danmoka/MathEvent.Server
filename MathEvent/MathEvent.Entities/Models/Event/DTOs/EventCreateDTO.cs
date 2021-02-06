using System;

namespace MathEvent.Entities.Models.Event.DTOs
{
    /// <summary>
    /// Класс для передачи данных для создания Event
    /// </summary>
    public class EventCreateDTO
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
