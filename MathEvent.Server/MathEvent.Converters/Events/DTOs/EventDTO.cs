using System;

namespace MathEvent.Converters.Events.DTOs
{
    /// <summary>
    /// Transfer объект сущности события
    /// </summary>
    public class EventDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public string AvatarPath { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        public int? ParentId { get; set; }
        #endregion

        public string Description { get; set; }

        public int? OrganizationId { get; set; }
    }
}
