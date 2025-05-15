using AutoMapper;
using System.Linq.Expressions;
using TruckDispatcherApi.Data;
using TruckDispatcherApi.Library;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class TruckService(IMapper mapper,
        IRepository<Truck> repository,
        IServiceResult<Truck> serviceResult) : AppBaseService<Truck, TruckDto>(mapper, repository, serviceResult), ITruckService
    {
        public async Task<TruckSearchParams<TruckDto>> GetAsync(TruckSearchParams<TruckDto> truckSearchParams)
        {
            // filtering
            var filters = new List<Expression<Func<Truck, bool>>> { t => t.UserId == truckSearchParams.UserId };
            if (!string.IsNullOrEmpty(truckSearchParams.SearchCriteria))
                filters.Add(t => t.Name.Contains(truckSearchParams.SearchCriteria)
                || t.LicensePlate.Contains(truckSearchParams.SearchCriteria));
            if (truckSearchParams.Equipment != Equipment.All)
                filters.Add(t => t.Equipment == truckSearchParams.Equipment);
            if (truckSearchParams.TruckStatus != TruckStatus.All)
                filters.Add(t => t.TruckStatus == truckSearchParams.TruckStatus);

            // Include only drivers, loads number is too much in short time
            var navProperties = new List<Expression<Func<Truck, object>>>();
            if (truckSearchParams.IncludeNavProperties) navProperties.Add(t => t.Drivers);

            // sorting by Name, Equipment, LicensePlate, CostPerMile or TruckStatus
            Func<IQueryable<Truck>, IOrderedQueryable<Truck>>? orderBy = null;
            if (truckSearchParams.Order != OrderType.None)
            {
                orderBy = truckSearchParams.SortField switch
                {
                    "Equipment" => truckSearchParams.Order == OrderType.Ascending ? q => q.OrderBy(t => t.Equipment) : q => q.OrderByDescending(t => t.Equipment),
                    "License Plate" => truckSearchParams.Order == OrderType.Ascending ? q => q.OrderBy(t => t.LicensePlate) : q => q.OrderByDescending(t => t.LicensePlate),
                    "Truck Status" => truckSearchParams.Order == OrderType.Ascending ? q => q.OrderBy(t => t.TruckStatus) : q => q.OrderByDescending(t => t.TruckStatus),
                    "Cost Per Mile" => truckSearchParams.Order == OrderType.Ascending ? q => q.OrderBy(t => t.CostPerMile) : q => q.OrderByDescending(t => t.CostPerMile),
                    _ => truckSearchParams.Order == OrderType.Ascending ? q => q.OrderBy(c => c.Name) : q => q.OrderByDescending(c => c.Name),
                };
            }

            await Search(truckSearchParams, filters, navProperties, orderBy);

            return truckSearchParams;
        }

        new public async Task<TruckDto?> GetAsync(string id)
        {
            Expression<Func<Truck, bool>> searchQuery = t => t.Id == id;
            List<Expression<Func<Truck, object>>> navProperties = [t => t.Loads.OrderByDescending(t => t.PickUp), t => t.Drivers];

            return Mapper.Map<TruckDto>(await Repository.GetAsync(searchQuery, navProperties));
        }

        public async Task<TrucksByStatus> GetTrucksNumbersByStatusAsync(string userId)
        {
            // Get all trucks
            var result = new SearchParams<TruckDto>()
            {
                CurrentPage = 1,
                PageSize = 10000,
                SearchCriteria = string.Empty,
                SortField = string.Empty,
                Order = OrderType.Ascending,
                ItemList = []
            };

            await Search(result, [t => t.UserId == userId], null, null);

            var trucksByStatus = new TrucksByStatus()
            {
                All = result.ItemList.Count(),
                TrucksOnRoad = 0,
                TrucksPending = 0,
                TrucksRepair = 0,
            };

            foreach (var truck in result.ItemList)
            {
                switch (truck.TruckStatus)
                {
                    case TruckStatus.OnRoad:
                        trucksByStatus.TrucksOnRoad++;
                        break;
                    case TruckStatus.Pending:
                        trucksByStatus.TrucksPending++;
                        break;
                    default:
                        trucksByStatus.TrucksRepair++;
                        break;
                }
            }

            return trucksByStatus;
        }
    }
}
