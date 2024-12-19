namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Model class for SMTP settings.
    /// </summary>
    public class SmtpOptions
    {
        /// <summary>
        /// The constant string of SMTP settings name.
        /// </summary>
        public const string ConfigName = "Smtp";

        /// <summary>
        /// The SMTP server host.
        /// </summary>
        public string Host { get; set; } = "";

        /// <summary>
        /// The port for the SMTP server.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The username for authentication to SMTP server.
        /// </summary>
        public string Username { get; set; } = "";

        /// <summary>
        /// The password for authentication to SMTP server.
        /// </summary>
        public string Password { get; set; } = "";

        /// <summary>
        /// Whether the SMTP server uses SSL for connectivity.
        /// </summary>
        public bool IsUseSsl { get; set; }

        /// <summary>
        /// The sender name used for sent emails.
        /// </summary>
        public string SenderName { get; set; } = "";
    }
}