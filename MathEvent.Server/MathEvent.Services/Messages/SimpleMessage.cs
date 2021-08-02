using MathEvent.Contracts;

namespace MathEvent.Services.Messages
{
    /// <summary>
    /// Класс простого сообщения
    /// </summary>
    public class SimpleMessage : IMessage
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public override string ToString()
        {
            return Code is null ? Message : $"{Code}, {Message}";
        }
    }
}
