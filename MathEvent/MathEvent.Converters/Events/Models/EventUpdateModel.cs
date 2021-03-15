using System;
using System.Collections.Generic;

namespace MathEvent.Converters.Events.Models
{
    /// <summary>
    /// Класс для передачи данных для обновления Event
    /// </summary>
    public class EventUpdateModel
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

        public ICollection<string> Managers { get; set; }

        public string Description { get; set; }

        public int? OrganizationId { get; set; }
    }
}
