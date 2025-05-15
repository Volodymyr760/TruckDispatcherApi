using AutoMapper;
using TruckDispatcherApi.Data;
using TruckDispatcherApi.Library;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class PricepackageService(IMapper mapper,
        IRepository<Pricepackage> repository,
        IServiceResult<Pricepackage> serviceResult) : AppBaseService<Pricepackage, PricepackageDto>(mapper, repository, serviceResult), IPricepackageService
    {
        public async Task<ISearchParams<PricepackageDto>> GetAsync(ISearchParams<PricepackageDto> searchParams)
        {
            // no filters

            // no navigation properties

            // sorting by Name, Price or Period
            Func<IQueryable<Pricepackage>, IOrderedQueryable<Pricepackage>>? orderBy = null;
            if (searchParams.Order != OrderType.None)
            {
                orderBy = searchParams.SortField switch
                {
                    "Name" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(p => p.Name) : q => q.OrderByDescending(p => p.Name),
                    "Price" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(p => p.Price) : q => q.OrderByDescending(p => p.Price),
                    _ => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(p => p.Period) : q => q.OrderByDescending(p => p.Period)
                };
            }

            await Search(searchParams, null, null, orderBy);

            return searchParams;
        }
    }
}
