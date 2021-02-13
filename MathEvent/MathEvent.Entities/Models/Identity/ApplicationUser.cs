using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MathEvent.Entities.Models.Identity
{
    /// <summary>
    /// Класс, расширяющий базовую модель пользователя
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Events = new HashSet<Event.Event>();
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public virtual ICollection<Event.Event> Events { get; set; }
    }
}
