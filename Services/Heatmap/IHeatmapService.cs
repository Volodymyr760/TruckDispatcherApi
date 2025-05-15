using TruckDispatcherApi.Library;
using TruckDispatcherApi.Services.Common;

namespace TruckDispatcherApi.Services
{
    public interface IHeatmapService: IBaseService<HeatmapDto>
    {
        Task<HeatmapSearchParams<HeatmapDto>> GetAsync(HeatmapSearchParams<HeatmapDto> searchParams);

        Task ChangeUpdatedATAsync(HeatmapDto heatmap);
    }
}
