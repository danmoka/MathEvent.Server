using MathEvent.Converters.Identities.DTOs;
using System;
using System.Collections.Generic;

namespace MathEvent.Converters.Events.DTOs
{
    /// <summary>
    /// Класс для передачи данных для чтения Event
    /// </summary>
    public class EventReadDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        public int? ParentId { get; set; }
        #endregion

        public ICollection<UserSimpleReadDTO> ApplicationUsers { get; set; }

        public string Description { get; set; }
    }
}
