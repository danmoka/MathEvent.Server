using MathEvent.Converters.Identities.Models;
using MathEvent.Converters.Organizations.Models;
using System;
using System.Collections.Generic;

namespace MathEvent.Converters.Events.Models
{
    /// <summary>
    /// Класс для передачи данных для чтения информации о событии
    /// </summary>
    public class EventWithUsersReadModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        public int? ParentId { get; set; }
        #endregion

        public ICollection<UserSimpleReadModel> ApplicationUsers { get; set; }

        public ICollection<UserSimpleReadModel> Managers { get; set; }

        public string Description { get; set; }

        public int? OwnerId { get; set; }

        public OrganizationReadModel Organization { get; set; }
    }
}
