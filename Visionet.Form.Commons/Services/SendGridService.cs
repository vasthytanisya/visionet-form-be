using Visionet.Form.Commons.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Serilog;

namespace Visionet.Form.Commons.Services
{
    public class SendGridService : IMailService
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly SendGridOptions _config;
        public SendGridService(ISendGridClient sendGridClient, IOptions<SendGridOptions> options)
        {
            _sendGridClient = sendGridClient;
            _config = options.Value;
        }

        public async Task<bool> SendMailAsync(SendMailModel mailContent)
        {
            if (mailContent.Tos.Count < 1)
            {
                throw new ArgumentException($"{nameof(SendMailModel.Tos)} must not be empty");
            }

            if (!mailContent.Subject.HasValue())
            {
                throw new ArgumentException($"{nameof(SendMailModel.Subject)} must not be empty");
            }

            if (!mailContent.Message.HasValue())
            {
                throw new ArgumentException($"{nameof(SendMailModel.Message)} must not be empty");
            }

            var sender = new EmailAddress(_config.Sender.Email, _config.Sender.Name);
            var recipients = CreateRecipients(mailContent.Tos);

            var subject = mailContent.Subject;
            var content = mailContent.Message;

            var message = MailHelper.CreateSingleEmailToMultipleRecipients(sender, recipients, subject, null, content);

            if (mailContent.Ccs.Count > 0)
            {
                foreach (var cc in mailContent.Ccs)
                {
                    message.AddCc(new EmailAddress(cc.Email, cc.Name));
                }
            }
            if (mailContent.Bccs.Count > 0)
            {
                foreach (var bcc in mailContent.Bccs)
                {
                    message.AddBcc(new EmailAddress(bcc.Email, bcc.Name));
                }
            }

            var response = await _sendGridClient.SendEmailAsync(message);

            if (!response.IsSuccessStatusCode)
            {
                Log.Error(await response.Body.ReadAsStringAsync());

                return false;
            }

            return true;
        }

        /// <summary>
        /// Create email recipients for SendGrid message object.
        /// </summary>
        /// <param name="recipients"></param>
        /// <returns></returns>
        private static List<EmailAddress> CreateRecipients(List<EmailData> recipients)
        {
            var emailAddresses = new List<EmailAddress>();

            foreach (var recipient in recipients)
            {
                emailAddresses.Add(new EmailAddress(recipient.Email, recipient.Name));
            }

            return emailAddresses;
        }
    }
}
