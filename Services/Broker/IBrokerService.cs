using TruckDispatcherApi.Services.Common;

namespace TruckDispatcherApi.Services
{
    public interface IBrokerService : IBaseService<BrokerDto>
    {
        Task<ISearchParams<BrokerDto>> GetAsync(ISearchParams<BrokerDto> searchParams);

        Task<BrokerDto?> GetByNameAsync(string brokerShortName);
    }
}
