using AutoMapper;
using System.Linq.Expressions;
using TruckDispatcherApi.Data;
using TruckDispatcherApi.Library;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class SubscriptionService(IMapper mapper,
        IRepository<Subscription> repository,
        IServiceResult<Subscription> serviceResult) : AppBaseService<Subscription, SubscriptionDto>(mapper, repository, serviceResult), ISubscriptionService
    {
        public async Task<ISearchParams<SubscriptionDto>> GetAsync(ISearchParams<SubscriptionDto> searchParams)
        {
            // filtering
            var filters = new List<Expression<Func<Subscription, bool>>>();
            if (!string.IsNullOrEmpty(searchParams.SearchCriteria))
                filters.Add(s => s.Title.Contains(searchParams.SearchCriteria));

            // sorting by Title, Content or CreatedAt
            Func<IQueryable<Subscription>, IOrderedQueryable<Subscription>>? orderBy = null;
            if (searchParams.Order != OrderType.None)
            {
                orderBy = searchParams.SortField switch
                {
                    "Content" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(s => s.Content) : q => q.OrderByDescending(s => s.Content),
                    "CreatedAt" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(s => s.CreatedAt) : q => q.OrderByDescending(s => s.CreatedAt),
                    _ => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(s => s.Title) : q => q.OrderByDescending(s => s.Title),
                };
            }

            await Search(searchParams, filters: filters, orderBy: orderBy);

            return searchParams;
        }
    }
}
