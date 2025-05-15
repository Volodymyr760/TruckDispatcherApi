using TruckDispatcherApi.Services.Common;

namespace TruckDispatcherApi.Services
{
    public interface ILoadService: IBaseService<LoadDto>
    {
        Task<LoadSearchParams<LoadDto>> GetAsync(LoadSearchParams<LoadDto> searchParams);

        Task<LoadsByStatus> GetLoadsNumbersByStatusAsync(string userId);

        Task<WeekResults> GetWeekResultsAsync(string userId);

        Task<EquipmentProfitability> GetEquipmentProfitabilityAsync(string userId);
    }
}
