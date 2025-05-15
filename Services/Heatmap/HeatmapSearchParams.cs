using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Services
{
    public class HeatmapSearchParams<HeatmapDto> : SearchParams<HeatmapDto> where HeatmapDto : class
    {
        public required string DayType { get; set; }

        public Equipment Equipment { get; set; }
    }
}
