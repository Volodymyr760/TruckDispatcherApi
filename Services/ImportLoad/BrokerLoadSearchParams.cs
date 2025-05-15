using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Services
{
    public class BrokerLoadSearchParams<ImportloadDto> : SearchParams<ImportloadDto> where ImportloadDto : class
    {
        public Equipment Equipment { get; set; }
    }
}
