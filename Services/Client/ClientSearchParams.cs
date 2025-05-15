using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Services
{
    public class ClientSearchParams<ClientDto> : SearchParams<ClientDto> where ClientDto : class
    {
        public ClientStatus ClientStatus { get; set; }

        public AppRoles AppRoles { get; set; }
    }
}
