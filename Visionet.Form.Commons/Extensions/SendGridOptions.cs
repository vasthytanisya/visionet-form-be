using Visionet.Form.Commons.Services;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Model class for binding the SendGrid appsettings configurations.
    /// </summary>
    public class SendGridOptions
    {
        /// <summary>
        /// The constant string of SendGrid settings name.
        /// </summary>
        public const string SendGrid = "SendGrid";

        /// <summary>
        /// Gets or sets the SendGrid API key.
        /// </summary>
        public string ApiKey { get; set; } = "";

        /// <summary>
        /// Gets or sets the SendGrid sender's email.
        /// </summary>
        public EmailData Sender { get; set; }
    }
}
