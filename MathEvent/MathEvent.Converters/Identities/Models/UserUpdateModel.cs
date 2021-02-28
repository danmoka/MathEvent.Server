using System.Collections.Generic;

namespace MathEvent.Converters.Identities.Models
{
    /// <summary>
    /// Класс для передачи данных для обновления пользователя
    /// </summary>
    public class UserUpdateModel
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string Patronymic { get; set; }

        public virtual ICollection<int> Events { get; set; }
    }
}
