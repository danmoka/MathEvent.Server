using MathEvent.Entities.Entities;

namespace MathEvent.Contracts
{
    /// <summary>
    /// Декларирует функциональность отправителя email сообщений
    /// </summary>
    public interface IEmailSender
    {
        IResult<IMessage, Message> SendEmail(Message message);
    }
}
