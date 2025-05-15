using System.ComponentModel.DataAnnotations;

namespace TruckDispatcherApi.Services
{
    public class HeatmapStateDto
    {
        public string? Id { get; set; }

        [Required]
        [StringLength(450)]
        public required string HeatmapId { get; set; }

        [Required]
        [StringLength(2)]
        public required string State { get; set; }

        [Required]
        public int PickupsAmount { get; set; }

        [Required]
        public decimal SumPickupRates { get; set; }

        [Required]
        public decimal AveragePickupRate { get; set; }

        [Required]
        public int DeliveriesAmount { get; set; }

        [Required]
        public decimal SumDeliveryRates { get; set; }

        [Required]
        public decimal AverageDeliveryRate { get; set; }

        [Required]
        public int Ranq { get; set; }
    }
}
