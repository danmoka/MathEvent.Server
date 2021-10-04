namespace MathEvent.Models.Users
{
    /// <summary>
    /// Класс для передачи данных для чтения информации о пользователе
    /// </summary>
    public class UserReadModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Patronymic { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }
    }
}
