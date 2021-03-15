namespace MathEvent.Converters.Identities.Models
{
    /// <summary>
    /// Класс для передачи данных для создания пользователя
    /// </summary>
    public class UserCreateModel
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }

        public int? OrganizationId { get; set; }
    }
}
