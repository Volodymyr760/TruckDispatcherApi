using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TruckDispatcherApi.Services
{
    public class SubscriberDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Email (1-256 characters) is required."), StringLength(256)]
        public required string Email { get; set; }

        [Column(TypeName = "datetime2(0)")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<SubscriberSubscriptionDto> SubscriberSubscriptions { get; set; } = [];
    }
}
