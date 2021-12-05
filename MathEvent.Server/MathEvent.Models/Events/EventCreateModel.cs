using System;

namespace MathEvent.Models.Events
{
    /// <summary>
    /// Класс для передачи данных для создания события
    /// </summary>
    public class EventCreateModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string StartDate { get; set; }

        public string Location { get; set; }

        public Guid AuthorId { get; set; }

        public int? OrganizationId { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        public int? ParentId { get; set; }
        #endregion
    }
}
