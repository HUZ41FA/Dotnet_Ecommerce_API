using ECommerce.Domain.Abstractions.IServices.Application;
using ECommerce.Infrastructure.Email;
using ECommerce.Utilities.Helper;
using MailKit.Net.Smtp;
using MimeKit;

namespace ECommerce.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfig _emailConfig;

        public EmailService(EmailConfig emailConfig)
        {
            _emailConfig = emailConfig;
        }
        public async Task SendMailAsync(string[] emailList, string subject, string content)
        {
            Message message = new Message(emailList, subject, content);
            MimeMessage emailMessage = CreateEmailMessage(message);
            await SendAsync(emailMessage);
        }
        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("42Tech.pk", _emailConfig.FromEmail));
            emailMessage.To.AddRange(message.ToEmailList);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };

            return emailMessage;
        }
        private async Task SendAsync(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_emailConfig.Username, _emailConfig.Password);

                await client.SendAsync(mailMessage);
            }
            catch
            {
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }
        }
    }
}
