using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Models
{
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string Id { get; set; }

        [Required(ErrorMessage = "Name (50 characters) is required."), StringLength(50)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Email is required."), StringLength(256)]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Location city is required."), StringLength(50)]
        public required string City { get; set; }

        [Required]
        public ClientStatus ClientStatus { get; set; }

        [Required]
        public AppRoles AppRoles { get; set; }

        [Required]
        [StringLength(20)]
        public required string DotNumber { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "datetime2(0)")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "datetime2(0)")]
        public DateTime? InvitedAt { get; set; }

        [StringLength(450)]
        public string? Notes { get; set; }
    }
}
