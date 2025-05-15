using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Models
{
    public class MailTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

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
