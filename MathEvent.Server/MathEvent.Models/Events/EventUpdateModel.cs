using System;
using System.Collections.Generic;

namespace MathEvent.Models.Events
{
    /// <summary>
    /// Класс для передачи данных для обновления события
    /// </summary>
    public class EventUpdateModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string StartDate { get; set; }

        public string Location { get; set; }

        public int? OrganizationId { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        public int? ParentId { get; set; }
        #endregion

        /// <summary>
        /// Для обновления коллекции пользователей по их id
        /// </summary>
        public ICollection<Guid> ApplicationUsers { get; set; }

        public ICollection<Guid> Managers { get; set; }
    }
}
