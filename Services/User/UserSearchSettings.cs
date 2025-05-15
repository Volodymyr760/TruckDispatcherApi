using System.ComponentModel.DataAnnotations;
using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Services
{
    public class UserSearchSettings
    {
        [StringLength(450)]
        public required string UserId { get; set; }

        public int Deadheads { get; set; }

        public int MilesMin { get; set; }

        public int MilesMax { get; set; }

        [Required(ErrorMessage = "SearchSortField (1-20 characters) is required."), StringLength(20)]
        public required string SortField { get; set; }

        [Required]
        public OrderType Sort { get; set; }
    }
}
