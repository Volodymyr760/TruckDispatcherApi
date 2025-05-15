using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TruckDispatcherApi.Models
{
    public class Subscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string Id { get; set; }

        [Required(ErrorMessage = "Title is required."), StringLength(450)]
        public required string Title { get; set; }

        [Required]
        public required string Content { get; set; }

        [Column(TypeName = "datetime2(0)")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Subscriber> Subscribers { get; set; } = [];
    }
}
