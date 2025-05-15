using TruckDispatcherApi.Services.Common;

namespace TruckDispatcherApi.Services
{
    public interface INotificationService : IBaseService<NotificationDto>
    {
        Task<ISearchParams<NotificationDto>> GetAsync(ISearchParams<NotificationDto> searchParams);

        Task NotifyAdmins(NotificationDto notificationDto);
    }
}
