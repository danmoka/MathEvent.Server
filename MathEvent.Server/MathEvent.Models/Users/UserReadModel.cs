using System;

namespace MathEvent.Models.Users
{
    /// <summary>
    /// Класс для передачи данных для чтения информации о пользователе
    /// </summary>
    public class UserReadModel
    {
        public Guid Id { get; set; }

        public Guid IdentityUserId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }
    }
}
