using System.Net.Mail;
using System.Text.RegularExpressions;

namespace TruckDispatcherApi.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration configuration;

        public EmailSender(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendEmailHtmlWithTemplate(string to, MailTemplateDto mailTemplate, string[] transformWith)
        {
            MailAddress mailFrom = new MailAddress(configuration["EmailSettings:EmailAddress"], "support@truckdispatcher.com");
            MailAddress mailTo = new MailAddress(to);
            string body = mailTemplate.MessageHtml;
            var regex = new Regex(Regex.Escape("{ph}"));

            foreach (var s in transformWith) body = regex.Replace(body, s, 1);

            MailMessage msg = new MailMessage(mailFrom, mailTo)
            {
                Subject = mailTemplate.Subject,
                Body = body,
                IsBodyHtml = true
            };

            SmtpClient client = new SmtpClient(configuration["EmailSettings:SmtpServer"], int.Parse(configuration["EmailSettings:SmtpPort"]))
            {
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(configuration["EmailSettings:SmtpUser"], configuration["EmailSettings:SmtpKey"])
            };

            await client.SendMailAsync(msg);
        }

        public async Task SendMailingListAsync(MailingListDto mailingListDto, MailTemplateDto mailTemplate)
        {
            foreach (string email in mailingListDto.EmailAddresses)
            {
                try
                {
                    if (IsValid(email))
                    {
                        await SendEmailHtmlWithTemplate(email, mailTemplate, new string[0]);
                        Thread.Sleep(13333);// 270 email/hour
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + ". Last sent email: " + email);
                }
            }
        }

        private bool IsValid(string emailAddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailAddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
