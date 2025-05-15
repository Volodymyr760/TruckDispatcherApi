using System.ComponentModel.DataAnnotations;
using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Services
{
    public class MailTemplateDto
    {
        public string? Id { get; set; }

        [Required]
        public MailTemplateKey MailTemplateKey { get; set; }

        [Required]
        public required string MessageHtml { get; set; }

        [Required]
        public required string MessagePlainText { get; set; }

        [Required]
        public required string Subject { get; set; }
    }
}
