namespace MathEvent.Contracts.Validators
{
    /// <summary>
    /// Декларирует функциональность ошибки валидации
    /// </summary>
    public interface IValidationError
    {
        string Field { get; set; }

        string Message { get; set; }
    }
}
