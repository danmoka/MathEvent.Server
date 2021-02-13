using MathEvent.Entities.Models.Events;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MathEvent.Entities.Models.Identities
{
    /// <summary>
    /// Класс, расширяющий базовую модель пользователя
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Events = new HashSet<Event>();
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }
}
