using System.ComponentModel.DataAnnotations;
using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Services
{
    public class TripDto
    {
        public int Id { get; set; }

        public string? OriginCity { get; set; }

        public string? DestinationCity { get; set; }

        public DateTime Start { get; set; }

        public DateTime Finish { get; set; }

        public double Deadheads { get; set; }

        public Equipment Equipment { get; set; }

        public bool IsTeam { get; set; } = false;

        public double Speed { get; set; } = 55;

        public List<ImportLoadDto> Loads { get; set; } = [];

        public double Miles { get; set; }

        public decimal Gross { get; set; }

        public decimal Costs { get; set; }

        public decimal RatePerMile { get; set; }

        public decimal Profit { get; set; }

        public decimal ProfitPerMile { get; set; }

        [StringLength(450)]
        public required string MapDirection { get; set; }
    }
}
