using System;

namespace MathEvent.Models.Users
{
    /// <summary>
    /// Класс для передачи данных для создания пользователя
    /// </summary>
    public class UserCreateModel
    {
        public Guid IdentityUserId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }
    }
}
