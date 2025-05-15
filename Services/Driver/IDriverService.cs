using TruckDispatcherApi.Services.Common;

namespace TruckDispatcherApi.Services
{
    public interface IDriverService: IBaseService<DriverDto>
    {
        Task<ISearchParams<DriverDto>> GetAsync(ISearchParams<DriverDto> searchParams);

        Task<IEnumerable<SearchDriverDto>> GetSearchDriversAsync(string name, string userId);

        Task RemoveAssignedTruckAsync(string driverId);
    }
}
