using MathEvent.Models.Email;

namespace MathEvent.Contracts.Services
{
    /// <summary>
    /// Декларирует функциональность отправителя email сообщений
    /// </summary>
    public interface IEmailService
    {
        // TODO: Email Service в отдельный проект, Message - в проект моделей
        void SendEmail(Message message);
    }
}
