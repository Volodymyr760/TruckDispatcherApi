using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckDispatcherApi.Services
{
    public class NotificationDto
    {
        public string? Id { get; set; }

        [StringLength(450)]
        public string? SenderAvatarUrl { get; set; }

        [Required, StringLength(50)]
        public required string SenderFullName { get; set; }

        [Required]
        public required string Message { get; set; }

        [Required]
        public bool IsRead { get; set; }

        public string? CallBackUrl { get; set; }

        [Column(TypeName = "datetime2(0)")]
        public DateTime CreatedAt { get; set; }

        [Required]
        public required string RecipientId { get; set; }

        [Required]
        public required string RecipientEmail { get; set; }
    }
}
