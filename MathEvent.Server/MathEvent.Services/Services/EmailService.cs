using MailKit.Net.Smtp;
using MathEvent.Contracts;
using MathEvent.Entities.Entities;
using MathEvent.Services.Results;
using MimeKit;
using System.Collections.Generic;

namespace MathEvent.Services.Services
{
    /// <summary>
    /// Сервис отправки email сообщений
    /// </summary>
    public class EmailService : IEmailSender
    {
        private readonly IEmailConfiguration _emailConfig;

        public EmailService(IEmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public IResult<IMessage, Message> SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);

            var sendResult = Send(emailMessage);

            if (sendResult.Succeeded)
            {
                return ResultFactory.GetSuccessfulResult(message);
            }
            else
            {
                return ResultFactory.GetUnsuccessfulMessageResult<Message>(sendResult.Messages);
            }
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

        private IResult<IMessage, MimeMessage> Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();

            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                client.Send(mailMessage);
            }
            catch
            {
                return ResultFactory.GetUnsuccessfulMessageResult<MimeMessage>(new List<IMessage>()
                {
                    MessageFactory.GetSimpleMessage(null, "Errors when sending email")
                });
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }

            return ResultFactory.GetSuccessfulResult(mailMessage);
        }
    }
}
