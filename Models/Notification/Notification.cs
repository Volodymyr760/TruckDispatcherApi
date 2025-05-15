using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TruckDispatcherApi.Models
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string Id { get; set; }

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

        [Required, StringLength(450)]
        public required string RecipientId { get; set; }

        [Required]
        public required string RecipientEmail { get; set; }
    }
}
