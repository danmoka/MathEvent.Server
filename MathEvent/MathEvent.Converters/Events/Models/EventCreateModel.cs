using System;

namespace MathEvent.Converters.Events.Models
{
    /// <summary>
    /// Класс для передачи данных для создания события
    /// </summary>
    public class EventCreateModel
    {
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        public int? ParentId { get; set; }
        #endregion

        public string Description { get; set; }

        public int? OrganizationId { get; set; }
    }
}
