using System.ComponentModel.DataAnnotations;

namespace MathEvent.Converters.Identities.Models
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Email пользователя должен быть задан")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
