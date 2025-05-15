using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;
using TruckDispatcherApi.Data;
using TruckDispatcherApi.Library;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class BrokerService(IMapper mapper,
        IRepository<Broker> repository,
        IServiceResult<Broker> serviceResult,
        IMemoryCache memoryCache) : AppBaseService<Broker, BrokerDto>(mapper, repository, serviceResult), IBrokerService
    {
        private readonly IMemoryCache cache = memoryCache;

        public async Task<ISearchParams<BrokerDto>> GetAsync(ISearchParams<BrokerDto> searchParams)
        {
            // filtering
            var filters = new List<Expression<Func<Broker, bool>>>();
            if (!string.IsNullOrEmpty(searchParams.SearchCriteria))
                filters.Add(b => b.Name.Contains(searchParams.SearchCriteria) || b.ShortName.Contains(searchParams.SearchCriteria));

            // No included entities

            // sorting by Name, Dot, MC or ShortName
            Func<IQueryable<Broker>, IOrderedQueryable<Broker>>? orderBy = null;
            if (searchParams.Order != OrderType.None)
            {
                orderBy = searchParams.SortField switch
                {
                    "Dot" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(b => b.DotNumber) : q => q.OrderByDescending(b => b.DotNumber),
                    "MC" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(b => b.McNumber) : q => q.OrderByDescending(b => b.McNumber),
                    "Name" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(b => b.Name) : q => q.OrderByDescending(b => b.Name),
                    _ => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(b => b.ShortName) : q => q.OrderByDescending(b => b.ShortName),
                };
            }

            await Search(searchParams, filters, null, orderBy);

            return searchParams;
        }

        public async Task<BrokerDto?> GetByNameAsync(string brokerShortName)
        {
            if (!cache.TryGetValue(brokerShortName, out BrokerDto? brokerDto))
            {
                Expression<Func<Broker, bool>> searchQuery = b => b.ShortName == brokerShortName;
                List<Expression<Func<Broker, object>>> navProperties = [];
                var brokerFromDb = await Repository.GetAsync(searchQuery, navProperties);

                if (brokerFromDb != null)
                {
                    brokerDto = Mapper.Map<BrokerDto>(brokerFromDb);
                    cache.Set(brokerShortName, brokerDto, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(20)));
                }
            }

            return brokerDto;
        }
    }
}
