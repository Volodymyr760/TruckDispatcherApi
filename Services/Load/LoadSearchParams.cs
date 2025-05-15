using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Services
{
    public class LoadSearchParams<LoadDto> : SearchParams<LoadDto> where LoadDto : class
    {
        public Equipment Equipment { get; set; }

        public LoadStatus LoadStatus { get; set; }

    }
}
