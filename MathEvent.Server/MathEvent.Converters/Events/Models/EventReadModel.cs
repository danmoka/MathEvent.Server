using System;

namespace MathEvent.Converters.Events.Models
{
    /// <summary>
    /// Класс для передачи информации о событии (без пользователей)
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
