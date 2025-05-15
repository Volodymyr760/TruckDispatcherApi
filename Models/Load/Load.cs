using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Models
{
    public class Load
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string Id { get; set; }

        [Required(ErrorMessage = "Reference Id is required."), StringLength(450)]
        public required string ReferenceId { get; set; }

        [Required(ErrorMessage = "Origin city is required."), StringLength(50)]
        public required string Origin { get; set; }

        [Required(ErrorMessage = "Destination city is required."), StringLength(50)]
        public required string Destination { get; set; }

        [Column(TypeName = "datetime2(0)")]
        public DateTime PickUp { get; set; }

        [Column(TypeName = "datetime2(0)")]
        public DateTime Delivery { get; set; }

        public int Length { get; set; }

        public int Weight { get; set; }

        [Required]
        public Equipment Equipment { get; set; }

        [Required(ErrorMessage = "Shipper name is required."), StringLength(256)]
        public required string ShipperName { get; set; }

        [StringLength(50)]
        public string? ShipperLogo { get; set; }

        [StringLength(256)]
        public string? ShipperEmail { get; set; }

        [StringLength(50)]
        public string? ShipperPhone { get; set; }

        /// <summary>
        /// Origin - Destination distance without deadheads
        /// </summary>
        [Required]
        public double Miles { get; set; }

        /// <summary>
        /// Start city - Origin deadhead miles
        /// </summary>
        [Required]
        public double DeadheadOrigin { get; set; }

        /// <summary>
        /// Destination - Final city deadhead miles
        /// </summary>
        [Required]
        public double DeadheadDestination { get; set; }

        /// <summary>
        /// Rate for origin - destination trip
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Rate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal RatePerMile { get; set; }

        /// <summary>
        /// This load Profit including deadhead miles
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Profit { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ProfitPerMile { get; set; }

        [StringLength(450)]
        public string Requirements { get; set; } = string.Empty;

        public LoadStatus LoadStatus { get; set; } = LoadStatus.Saved;

        [StringLength(450)]
        public string? TruckId { get; set; }

        /// <summary>
        /// Assigned to truck (if any). Nonrequired nav link to Truck entity
        /// </summary>
        public Truck? Truck { get; set; }

        [StringLength(450)]
        public string? UserId { get; set; }
    }
}
