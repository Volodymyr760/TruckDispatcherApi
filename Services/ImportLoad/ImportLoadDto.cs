using System.ComponentModel.DataAnnotations;
using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Services
{
    public class ImportLoadDto
    {
        public string? Id { get; set; }

        public required string ReferenceId { get; set; }

        public required string Origin { get; set; }
        public required double OriginLatitude { get; set; }
        public required double OriginLongitude { get; set; }

        public required string Destination { get; set; }
        public required double DestinationLatitude { get; set; }
        public required double DestinationLongitude { get; set; }

        public DateTime PickUp { get; set; }

        public DateTime Delivery { get; set; }

        public int Length { get; set; }
        public int Weight { get; set; }
        public Equipment Equipment { get; set; }

        [StringLength(450)]
        public string? ShipperId { get; set; }

        [StringLength(256)]
        public required string ShipperName { get; set; }

        [StringLength(256)]
        public string? ShipperEmail { get; set; }

        [StringLength(50)]
        public string? ShipperPhone { get; set; }

        [StringLength(50)]
        public string? ShipperLogo { get; set; }

        public required string ShipperDotNumber { get; set; }
        public required string ShipperMcNumber { get; set; }

        public double Miles { get; set; }
        public double DeadheadOrigin { get; set; }
        public double DeadheadDestination { get; set; }
        public decimal Rate { get; set; }
        public decimal RatePerMile { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitPerMile { get; set; }

        public string Requirements { get; set; } = string.Empty;
    }
}
