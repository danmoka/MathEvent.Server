
namespace MathEvent.Contracts
{
    /// <summary>
    /// Описывает функционал сообщения
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Код (может использоваться для передачи кода ошибки)
        /// </summary>
        string Code { get; }

        /// <summary>
        /// Сообщение
        /// </summary>
        string Message { get; }
    }
}
