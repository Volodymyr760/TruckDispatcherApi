using TruckDispatcherApi.Library;
using TruckDispatcherApi.Models;
using TruckDispatcherApi.Services.Common;

namespace TruckDispatcherApi.Services
{
    public interface IImportLoadService : IBaseService<ImportLoadDto>
    {
        Task ChangePickupAndDeliveryDatesForTestsAsync(int days);

        Task DeleteLegacy();

        Task<ISearchParams<ImportLoadDto>> GetAsync(BrokerLoadSearchParams<ImportLoadDto> searchParams);

        Task<List<ImportLoadDto>> GetAsync(DateTime startDateTime, DateTime endDateTime, Equipment equipment);

        Task<AverageRates> GetAverageRatesAsync();

        Task<ImportLoad?> IsImportLoadExistsAsync(ImportLoadDto importLoadDto);

        Task<ISearchParams<ImportLoadDto>> SearchAsync(ImportLoadSearchParams<ImportLoadDto> searchParams);
    }
}
