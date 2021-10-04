using MathEvent.Entities.Entities;

namespace MathEvent.Contracts.Services
{
    /// <summary>
    /// Декларирует функциональность отправителя email сообщений
    /// </summary>
    public interface IEmailSender
    {
        void SendEmail(Message message);
    }
}
