using MathEvent.Contracts;

namespace Service.Messages
{
    /// <summary>
    /// Класс простого сообщения
    /// </summary>
    public class SimpleMessage : IMessage
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
