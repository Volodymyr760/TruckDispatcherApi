using TruckDispatcherApi.Services.Common;

namespace TruckDispatcherApi.Services
{
    public interface ITruckService: IBaseService<TruckDto>
    {
        Task<TruckSearchParams<TruckDto>> GetAsync(TruckSearchParams<TruckDto> truckSearchParams);

        Task<TrucksByStatus> GetTrucksNumbersByStatusAsync(string userId);
    }
}
