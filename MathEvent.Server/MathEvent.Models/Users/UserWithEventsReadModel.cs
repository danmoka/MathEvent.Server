using MathEvent.Models.Events;
using MathEvent.Models.Organizations;
using System;
using System.Collections.Generic;

namespace MathEvent.Models.Users
{
    /// <summary>
    /// Класс для передачи данных для чтения информации о пользователе и его событиях
    /// </summary>
    public class UserWithEventsReadModel
    {
        public Guid Id { get; set; }

        public Guid IdentityUserId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public int? OwnerId { get; set; }

        public OrganizationReadModel Organization { get; set; }

        public ICollection<EventReadModel> Events { get; set; }

        public ICollection<EventReadModel> ManagedEvents { get; set; }
    }
}
