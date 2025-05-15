using TruckDispatcherApi.Services.Common;

namespace TruckDispatcherApi.Services
{
    public interface IClientService : IBaseService<ClientDto>
    {
        Task<ClientSearchParams<ClientDto>> GetAsync(ClientSearchParams<ClientDto> clientSearchParams);
    }
}
