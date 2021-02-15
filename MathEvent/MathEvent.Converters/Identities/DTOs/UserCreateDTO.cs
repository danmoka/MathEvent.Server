
namespace MathEvent.Converters.Identities.DTOs
{
    /// <summary>
    /// Класс для передачи данных для создания пользователя
    /// </summary>
    public class UserCreateDTO
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }
    }
}
