using AutoMapper;
using System.Linq.Expressions;
using TruckDispatcherApi.Data;
using TruckDispatcherApi.Library;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class SubscriberService(IMapper mapper,
        IRepository<Subscriber> repository,
        IServiceResult<Subscriber> serviceResult) : AppBaseService<Subscriber, SubscriberDto>(mapper, repository, serviceResult), ISubscriberService
    {
        public async Task<ISearchParams<SubscriberDto>> GetAsync(ISearchParams<SubscriberDto> searchParams)
        {
            // filtering by part of Email field
            var filters = new List<Expression<Func<Subscriber, bool>>>();
            if (!string.IsNullOrEmpty(searchParams.SearchCriteria))
                filters.Add(s => s.Email.Contains(searchParams.SearchCriteria));

            // sorting by Email or CreatedAt
            Func<IQueryable<Subscriber>, IOrderedQueryable<Subscriber>>? orderBy = null;
            if (searchParams.Order != OrderType.None)
            {
                orderBy = searchParams.SortField switch
                {
                    "CreatedAt" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(s => s.CreatedAt) : q => q.OrderByDescending(s => s.CreatedAt),
                    _ => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(s => s.Email) : q => q.OrderByDescending(s => s.Email),
                };
            }

            await Search(searchParams, filters: filters, orderBy: orderBy);

            return searchParams;
        }
    }
}
