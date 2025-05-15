using TruckDispatcherApi.Services.Common;

namespace TruckDispatcherApi.Services
{
    public interface IPricepackageService: IBaseService<PricepackageDto>
    {
        Task<ISearchParams<PricepackageDto>> GetAsync(ISearchParams<PricepackageDto> searchParams);
    }
}
