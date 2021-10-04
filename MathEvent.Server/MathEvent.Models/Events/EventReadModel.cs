using System;

namespace MathEvent.Models.Events
{
    /// <summary>
    /// Класс для передачи упрощенной информации о событии
    /// </summary>
    public class EventReadModel
    {
        public int Id { get; set; }

        public string AvatarPath { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        public int? ParentId { get; set; }
        #endregion
    }
}
