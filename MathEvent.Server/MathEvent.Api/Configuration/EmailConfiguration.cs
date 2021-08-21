using MathEvent.Contracts;

namespace MathEvent.Api.Configuration
{
    /// <summary>
    /// Конфигурация отправки email сообщений
    /// </summary>
    public class EmailConfiguration : IEmailConfiguration
    {
        public string From { get; set; }

        public string SmtpServer { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
