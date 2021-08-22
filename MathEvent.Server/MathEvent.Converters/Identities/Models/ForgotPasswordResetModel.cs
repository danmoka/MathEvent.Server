using System.ComponentModel.DataAnnotations;

namespace MathEvent.Converters.Identities.Models
{
    public class ForgotPasswordResetModel
    {
        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "Email пользователя должен быть задан")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль пользователя должен быть задан")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Длина пароля пользователя должна быть от 1 до 255 символов")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтверждение пароля пользователя должено быть задано")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Длина подтверждения пароля пользователя должна быть от 1 до 255 символов")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; }
    }
}
