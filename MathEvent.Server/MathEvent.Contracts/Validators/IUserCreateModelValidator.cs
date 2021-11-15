using MathEvent.Models.Users;

namespace MathEvent.Contracts.Validators
{
    /// <summary>
    /// Декларирует функциональность валидатора модели создания пользователя
    /// </summary>
    public interface IUserCreateModelValidator : IValidator<UserCreateModel>
    {
    }
}
