using MathEvent.Models.Users;

namespace MathEvent.Contracts.Validators
{
    /// <summary>
    /// Декларирует функциональность валидатора модели смены пароля
    /// </summary>
    public interface IForgotPasswordModelValidator : IValidator<ForgotPasswordModel>
    {
    }
}
