using System;
using System.Collections.Generic;

namespace MathEvent.Models.Users
{
    /// <summary>
    /// Класс для передачи данных для обновления пользователя
    /// </summary>
    public class UserUpdateModel
    {
        public Guid IdentityUserId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public int? OrganizationId { get; set; }

        public virtual ICollection<int> Events { get; set; }

        public virtual ICollection<int> ManagedEvents { get; set; }
    }
}
