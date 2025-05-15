using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Services
{
    public class TruckSearchParams<TruckDto> : SearchParams<TruckDto> where TruckDto : class
    {
        public Equipment Equipment { get; set; }

        public TruckStatus TruckStatus { get; set; }
    }
}
