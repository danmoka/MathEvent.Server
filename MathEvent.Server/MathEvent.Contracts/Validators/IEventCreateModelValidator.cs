using MathEvent.Models.Events;

namespace MathEvent.Contracts.Validators
{
    /// <summary>
    /// Декларирует функциональность валидатора модели создания события
    /// </summary>
    public interface IEventCreateModelValidator : IValidator<EventCreateModel>
    {
    }
}
