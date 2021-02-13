using System;
using System.Collections.Generic;

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

        /// <summary>
        /// Для обновления коллекции пользователей по их id
        /// </summary>
        public ICollection<string> ApplicationUsers { get; set; }

        public string Description { get; set; }
    }
}
