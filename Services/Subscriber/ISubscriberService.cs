using TruckDispatcherApi.Services.Common;

namespace TruckDispatcherApi.Services
{
    public interface ISubscriberService : IBaseService<SubscriberDto>
    {
        Task<ISearchParams<SubscriberDto>> GetAsync(ISearchParams<SubscriberDto> searchParams);
    }
}
