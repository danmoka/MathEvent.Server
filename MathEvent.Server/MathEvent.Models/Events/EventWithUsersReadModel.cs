using MathEvent.Models.Organizations;
using MathEvent.Models.Users;
using System;
using System.Collections.Generic;

namespace MathEvent.Models.Events
{
    /// <summary>
    /// Класс для передачи данных для чтения информации о событии
    /// </summary>
    public class EventWithUsersReadModel
    {
        public int Id { get; set; }

        public string AvatarPath { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public OrganizationReadModel Organization { get; set; }

        public int? OwnerId { get; set; }

        #region hierarchy
        public bool? Hierarchy { get; set; }

        public int? ParentId { get; set; }
        #endregion

        public ICollection<UserReadModel> ApplicationUsers { get; set; }

        public ICollection<UserReadModel> Managers { get; set; }
    }
}
