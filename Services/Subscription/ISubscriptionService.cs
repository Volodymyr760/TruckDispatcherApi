using TruckDispatcherApi.Services.Common;

namespace TruckDispatcherApi.Services
{
    public interface ISubscriptionService: IBaseService<SubscriptionDto>
    {
        Task<ISearchParams<SubscriptionDto>> GetAsync(ISearchParams<SubscriptionDto> searchParams);
    }
}
