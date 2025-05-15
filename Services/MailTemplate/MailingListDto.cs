using System.ComponentModel.DataAnnotations;
using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Services
{
    public class MailingListDto
    {
        /// <summary>
        /// List of emails
        /// </summary>
        [Required(ErrorMessage = "List of emails is required.")]
        public required List<string> EmailAddresses { get; set; }

        /// <summary>
        /// MailTemplateKey to find MailTemplate in database
        /// </summary>
        [Required(ErrorMessage = "MailTemplateKey is required.")]
        public MailTemplateKey MailTemplateKey { get; set; }
    }
}
