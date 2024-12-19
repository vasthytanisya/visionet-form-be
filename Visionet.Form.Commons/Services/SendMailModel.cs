namespace Visionet.Form.Commons.Services
{
    /// <summary>
    /// Model class for sending mail.
    /// </summary>
    public class SendMailModel
    {
        /// <summary>
        /// Gets or sets the list of primary recipients (the "To" email field).
        /// </summary>
        public List<EmailData> Tos { get; set; } = new List<EmailData>();

        /// <summary>
        /// Get or sets the list of recipients for carbon copy (the "Cc" email field).
        /// </summary>
        public List<EmailData> Ccs { get; set; } = new List<EmailData>();

        /// <summary>
        /// Gets or sets the list of recipients for blind carbon copy (the "Bcc" email field).
        /// 
        /// </summary>
        public List<EmailData> Bccs { get; set; } = new List<EmailData>();

        /// <summary>
        /// Gets or sets the subject of the email.
        /// </summary>
        public string Subject { get; set; } = "";

        /// <summary>
        /// Gets or sets the contents / body of the email.
        /// </summary>
        public string Message { get; set; } = "";
    }

    /// <summary>
    /// Model class for binding the mail service sender's and recipients' email address object.<para></para>
    /// This model class mimick the SendGrid's EmailAddress class.
    /// </summary>
    public class EmailData
    {
/*        public EmailData()
        {

        }

        public EmailData(string email, string? name)
        {
            Email = email;
            Name = name;
        }
*/
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// Gets or sets the sender's / recipient's name.
        /// </summary>
        public string Name { get; set; } = "";
    }
}