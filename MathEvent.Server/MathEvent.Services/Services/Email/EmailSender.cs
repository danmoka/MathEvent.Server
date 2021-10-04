using MailKit.Net.Smtp;
using MathEvent.Contracts;
using MathEvent.Contracts.Services;
using MathEvent.Entities.Entities;
using MimeKit;

namespace MathEvent.Services.Services.Email
{
    /// <summary>
    /// Сервис отправки email сообщений
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly IEmailConfiguration _emailConfig;

        public EmailSender(IEmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public void SendEmail(Message message)
        {
            Send(CreateEmailMessage(message));
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };

            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();

            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                client.Send(mailMessage);
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
