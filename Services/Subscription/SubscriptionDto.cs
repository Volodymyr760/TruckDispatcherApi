using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class SubscriptionDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Title is required."), StringLength(450)]
        public required string Title { get; set; }

        [Required]
        public required string Content { get; set; }

        [Column(TypeName = "datetime2(0)")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<SubscriberSubscription> SubscriberSubscriptions { get; set; } = [];
    }
}
