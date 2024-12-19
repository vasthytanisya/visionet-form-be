using MailKit.Net.Smtp;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Visionet.Form.Commons.Services
{
    public class SmtpMailService : IMailService
    {
        private readonly SmtpOptions _config;

        public SmtpMailService(IOptions<SmtpOptions> options)
        {
            _config = options.Value;
        }

        public async Task<bool> SendMailAsync(SendMailModel mailContent)
        {
            var mimeMessage = new MimeMessage();

            mimeMessage.From.Add(new MailboxAddress(_config.SenderName, _config.Username));

            foreach (var to in mailContent.Tos)
            {
                mimeMessage.To.Add(MailboxAddress.Parse(to.Email));
            }

            foreach (var cc in mailContent.Ccs)
            {
                mimeMessage.Cc.Add(MailboxAddress.Parse(cc.Email));
            }

            foreach (var bcc in mailContent.Bccs)
            {
                mimeMessage.Bcc.Add(MailboxAddress.Parse(bcc.Email));
            }

            mimeMessage.Subject = mailContent.Subject;

            mimeMessage.Body = new TextPart("html")
            {
                Text = mailContent.Message
            };

            using var client = new SmtpClient();

            //disable soon !
            client.CheckCertificateRevocation = false;

            await client.ConnectAsync(_config.Host, _config.Port, _config.IsUseSsl);

            await client.AuthenticateAsync(_config.Username, _config.Password);

            await client.SendAsync(mimeMessage);

            await client.DisconnectAsync(true);

            return true;
        }
    }
}
