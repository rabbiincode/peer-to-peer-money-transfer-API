using MailKit.Net.Smtp;
using MimeKit;
using peer_to_peer_money_transfer.Shared.Interfaces;

namespace peer_to_peer_money_transfer.Shared.EmailConfiguration
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        const string image = "https://firebasestorage.googleapis.com/v0/b/image-store-3e6e0.appspot.com/o/CashMingle.png?alt=media&token=aff6cd82-94ba-4a03-8b32-e7c414f7fffe";
        public async Task SendEmailAsync(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            await SendAsync(emailMessage);
        }
         
        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("CashMingle", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = string.Format(
                                "<h4 style='color:red;'>{0}</h4> <img src={1} alt='CashMingle' style='display:block;margin:0 auto;' width='100'/>", message.Content, image) };

            return emailMessage;
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

                    client.Send(mailMessage);
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
}
