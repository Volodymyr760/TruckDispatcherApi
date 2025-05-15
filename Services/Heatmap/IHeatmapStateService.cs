using TruckDispatcherApi.Services.Common;

namespace TruckDispatcherApi.Services
{
    public interface IHeatmapStateService : IBaseService<HeatmapStateDto>
    {
        void GenerateHeatmapStatesAsync(HeatmapDto heatmap, IEnumerable<ImportLoadDto> importLoads);

        /// <summary>
        /// Sets all HeatmapStates to default values
        /// </summary>
        /// <returns></returns>
        Task ResetAsync();

        Task SaveHeatmapStateAsync(HeatmapStateDto heatmapState);
    }
}
