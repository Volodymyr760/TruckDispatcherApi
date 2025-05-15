using AutoMapper;
using System.Data;
using System.Linq.Expressions;
using TruckDispatcherApi.Data;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class NotificationService(IConfiguration configuration, IMapper mapper,
            IRepository<Notification> repository,
            IServiceResult<Notification> serviceResult) : AppBaseService<Notification, NotificationDto>(mapper, repository, serviceResult), INotificationService
    {
        private readonly IConfiguration configuration = configuration;

        public async Task<ISearchParams<NotificationDto>> GetAsync(ISearchParams<NotificationDto> searchParams)
        {
            // filtering only by UserId
            var filters = new List<Expression<Func<Notification, bool>>> { n => n.RecipientId == searchParams.UserId };

            // sorting only by CreatedAt
            Func<IQueryable<Notification>, IOrderedQueryable<Notification>> orderBy = q => q.OrderByDescending(n => n.CreatedAt);

            await Search(searchParams, filters: filters, orderBy: orderBy);

            return searchParams;
        }

        public async Task NotifyAdmins(NotificationDto notificationDto)
        {
            var valuesSection = configuration.GetSection("AdminGroup");
            foreach (IConfigurationSection section in valuesSection.GetChildren())
            {
                notificationDto.Message = "Dear " + section.GetValue<string>("fullName") + ". " + notificationDto.Message;
                notificationDto.RecipientId = section.GetValue<string>("id");
                notificationDto.RecipientEmail = section.GetValue<string>("email");
                await CreateAsync(notificationDto);
            }
        }
    }
}
