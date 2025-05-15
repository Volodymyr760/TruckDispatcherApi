using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TruckDispatcherApi.Library;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class HeatmapDto
    {
        public string? Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string DayType { get; set; }

        [Required]
        public Equipment Equipment { get; set; }

        public List<HeatmapStateDto> HeatmapStates { get; set; } = [];

        [Required(ErrorMessage = "UpdatedAt is required")]
        public DateTime UpdatedAt { get; set; }
    }
}
