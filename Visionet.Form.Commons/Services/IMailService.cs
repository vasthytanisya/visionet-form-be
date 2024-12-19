namespace Visionet.Form.Commons.Services
{
    /// <summary>
    /// Interface definition for mail service.
    /// </summary>
    public interface IMailService
    {
        /// <summary>
        /// Sends mail asynchronously.
        /// </summary>
        /// <param name="mailContent">The content in <seealso cref="SendMailModel"/> format.</param>
        /// <returns></returns>
        Task<bool> SendMailAsync(SendMailModel mailContent);
    }
}
