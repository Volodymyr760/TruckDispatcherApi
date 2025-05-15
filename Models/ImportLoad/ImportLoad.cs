using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Models
{
    public class ImportLoad
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string Id { get; set; }

        [Required(ErrorMessage = "Reference Id is required."), StringLength(450)]
        public required string ReferenceId { get; set; }

        [Required(ErrorMessage = "Origin city is required."), StringLength(50)]
        public required string Origin { get; set; }

        [Required]
        public required double OriginLatitude { get; set; }

        [Required]
        public required double OriginLongitude { get; set; }

        [Required(ErrorMessage = "Destination city is required."), StringLength(50)]
        public required string Destination { get; set; }

        [Required]
        public required double DestinationLatitude { get; set; }

        [Required]
        public required double DestinationLongitude { get; set; }

        [Column(TypeName = "datetime2(0)")]
        public DateTime PickUp { get; set; }

        [Column(TypeName = "datetime2(0)")]
        public DateTime Delivery { get; set; }

        public int Length { get; set; }

        public int Weight { get; set; }

        [Required]
        public Equipment Equipment { get; set; }

        [StringLength(450)]
        public string? ShipperId { get; set; }

        [StringLength(256)]
        public string? ShipperEmail { get; set; }

        [StringLength(50)]
        public string? ShipperPhone { get; set; }

        [Required(ErrorMessage = "Shipper name is required."), StringLength(256)]
        public required string ShipperName { get; set; }

        [StringLength(50)]
        public string? ShipperLogo { get; set; }

        //[Required(ErrorMessage = "DOT number is required."), StringLength(20)]
        public required string ShipperDotNumber { get; set; }

        [Required(ErrorMessage = "MC number is required."), StringLength(20)]
        public required string ShipperMcNumber { get; set; }

        /// <summary>
        /// Origin - Destination distance without deadheads
        /// </summary>
        [Required]
        public double Miles { get; set; }

        [Required]
        public double DeadheadOrigin { get; set; }

        [Required]
        public double DeadheadDestination { get; set; }

        /// <summary>
        /// Rate for origin - destination trip
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Rate { get; set; }

        /// <summary>
        /// This load RPM including deadhead miles
        /// </summary>
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
    }
}
