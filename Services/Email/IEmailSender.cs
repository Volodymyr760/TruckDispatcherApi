namespace TruckDispatcherApi.Services
{
    public interface IEmailSender
    {
        /// <summary>
        /// Sends email using MailTemplate
        /// </summary>
        /// <param name="to">Recipient's email</param>
        /// <param name="mailTemplate">MailTemplateDto from db</param>
        /// <param name="transformWith">Array of values to replace placeholders into in MailTemplate.Subject and MessageHtml</param>
        /// <returns></returns>
        Task SendEmailHtmlWithTemplate(string to, MailTemplateDto mailTemplate, string[] transformWith);

        /// <summary>
        /// Sends the email to each recipient from mailingListDto.EmailAddresses using template by mailingListDto.MailTemplateKey
        /// </summary>
        /// <param name="mailingListDto">MailTemplateDto from db</param>
        /// <param name="mailTemplate">MailTemplateDto from db</param>
        /// <returns></returns>
        Task SendMailingListAsync(MailingListDto mailingListDto, MailTemplateDto mailTemplate);
    }
}
