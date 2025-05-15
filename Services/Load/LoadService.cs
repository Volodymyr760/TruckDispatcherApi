using AutoMapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq.Expressions;
using TruckDispatcherApi.Data;
using TruckDispatcherApi.Library;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class LoadService(IMapper mapper,
        IRepository<Load> repository,
        IServiceResult<Load> serviceResult) : AppBaseService<Load, LoadDto>(mapper, repository, serviceResult), ILoadService
    {
        public async Task<LoadSearchParams<LoadDto>> GetAsync(LoadSearchParams<LoadDto> searchParams)
        {
            // filtering by Origin or Destination cities
            var filters = new List<Expression<Func<Load, bool>>>() { l => l.UserId == searchParams.UserId };
            if (!string.IsNullOrEmpty(searchParams.SearchCriteria))
                filters.Add(l => l.Origin.Contains(searchParams.SearchCriteria)
                    || l.Destination.Contains(searchParams.SearchCriteria)
                    || l.ReferenceId.Contains(searchParams.SearchCriteria)
                    || l.Truck.Name.Contains(searchParams.SearchCriteria)
                    || l.Truck.LicensePlate.Contains(searchParams.SearchCriteria));
            if (searchParams.Equipment != Equipment.All)
                filters.Add(l => l.Equipment == searchParams.Equipment);
            if (searchParams.LoadStatus != LoadStatus.All)
                filters.Add(l => l.LoadStatus == searchParams.LoadStatus);

            // Include trucks
            var navProperties = new List<Expression<Func<Load, object>>>();
            if (searchParams.IncludeNavProperties) { navProperties.Add(l => l.Truck); }

            // sorting by Origin, Destination, PickUp, Delivery, Equipment, ShipperName, Miles, Rate, RatePerMile,
            // Profit, ProfitPerMile or LoadStatus
            Func<IQueryable<Load>, IOrderedQueryable<Load>>? orderBy = null;
            if (searchParams.Order != OrderType.None)
            {
                orderBy = searchParams.SortField switch
                {
                    "Origin" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.Origin) : q => q.OrderByDescending(l => l.Origin),
                    "Destination" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.Destination) : q => q.OrderByDescending(l => l.Destination),
                    "Delivery" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.Delivery) : q => q.OrderByDescending(l => l.Delivery),
                    "Equipment" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.Equipment) : q => q.OrderByDescending(l => l.Equipment),
                    "ShipperName" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.ShipperName) : q => q.OrderByDescending(l => l.ShipperName),
                    "LoadStatus" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.LoadStatus) : q => q.OrderByDescending(l => l.LoadStatus),
                    "Miles" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.Miles) : q => q.OrderByDescending(l => l.Miles),
                    "Rate" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.Rate) : q => q.OrderByDescending(l => l.Rate),
                    "Rate Per Mile" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.RatePerMile) : q => q.OrderByDescending(l => l.RatePerMile),
                    "Profit" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.Profit) : q => q.OrderByDescending(l => l.Profit),
                    "Profit Per Mile" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.ProfitPerMile) : q => q.OrderByDescending(l => l.ProfitPerMile),
                    "Truck Name" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.Truck.Name).ThenByDescending(l => l.PickUp) : orderBy = q => q.OrderByDescending(l => l.Truck.Name).ThenByDescending(l => l.PickUp),
                    _ => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.PickUp) : q => q.OrderByDescending(l => l.PickUp),
                };
            }

            await Search(searchParams, filters, navProperties, orderBy);

            return searchParams;
        }

        new public async Task<LoadDto?> GetAsync(string id)
        {
            Expression<Func<Load, bool>> searchQuery = d => d.Id == id;
            List<Expression<Func<Load, object>>> navProperties = [d => d.Truck];

            return Mapper.Map<LoadDto>(await Repository.GetAsync(searchQuery, navProperties));
        }

        public async Task<EquipmentProfitability> GetEquipmentProfitabilityAsync(string userId)
        {
            // Get all loads by userId
            var result = new SearchParams<LoadDto>()
            {
                CurrentPage = 1,
                PageSize = 10000,
                SearchCriteria = string.Empty,
                SortField = string.Empty,
                Order = OrderType.Ascending,
                ItemList = []
            };

            await Search(result, new List<Expression<Func<Load, bool>>> { l => l.UserId == userId }, null, null);


            decimal allGross = 0, allProfit = 0, flatbedGross = 0, flatbedProfit = 0,
                reeferGross = 0, reeferProfit = 0, vanGross = 0, vanProfit = 0;

            foreach (var load in result.ItemList)
            {
                allGross += load.Rate;
                allProfit += load.Profit;

                switch (load.Equipment)
                {
                    case Equipment.Flatbed:
                        flatbedGross += load.Rate;
                        flatbedProfit += load.Profit;
                        break;
                    case Equipment.Reefer:
                        reeferGross += load.Rate;
                        reeferProfit += load.Profit;
                        break;
                    default:
                        vanGross += load.Rate;
                        vanProfit += load.Profit;
                        break;
                }
            }

            var equipmentProfitability = new EquipmentProfitability()
            {
                All = allGross > 0 ? decimal.Round(allProfit / allGross * 100, 2) : 0,
                Flatbed = flatbedGross > 0 ? decimal.Round(flatbedProfit / flatbedGross * 100, 2) : 0,
                Reefer = reeferGross > 0 ? decimal.Round(reeferProfit / reeferGross * 100, 2) : 0,
                Van = vanGross > 0 ? decimal.Round(vanProfit / vanGross * 100, 2) : 0
            };

            return equipmentProfitability;
        }

        public async Task<LoadsByStatus> GetLoadsNumbersByStatusAsync(string userId)
        {
            // Get all loads by userId
            var result = new SearchParams<LoadDto>()
            {
                CurrentPage = 1,
                PageSize = 10000,
                SearchCriteria = string.Empty,
                SortField = string.Empty,
                Order = OrderType.Ascending,
                ItemList = []
            };

            await Search(result, new List<Expression<Func<Load, bool>>> { l => l.UserId == userId }, null, null);

            var loadsByStatus = new LoadsByStatus()
            {
                All = result.ItemList.Count()
            };

            foreach (var load in result.ItemList)
            {
                loadsByStatus.AllLoadsMileage += load.Miles + load.DeadheadOrigin + load.DeadheadDestination;
                loadsByStatus.AllLoadsGross += load.Rate;
                loadsByStatus.AllLoadsCosts += load.Rate - load.Profit;

                switch (load.LoadStatus)
                {
                    case LoadStatus.Saved:
                        loadsByStatus.SavedLoads++;
                        loadsByStatus.SavedLoadsMileage += load.Miles + load.DeadheadOrigin + load.DeadheadDestination;
                        loadsByStatus.SavedLoadsGross += load.Rate;
                        loadsByStatus.SavedLoadsCosts += load.Rate - load.Profit;
                        break;
                    case LoadStatus.Booked:
                        loadsByStatus.BookedLoads++;
                        loadsByStatus.BookedLoadsMileage += load.Miles + load.DeadheadOrigin + load.DeadheadDestination;
                        loadsByStatus.BookedLoadsGross += load.Rate;
                        loadsByStatus.BookedLoadsCosts += load.Rate - load.Profit;
                        break;
                    case LoadStatus.InProgress:
                        loadsByStatus.InProgressLoads++;
                        loadsByStatus.InProgressLoadsMileage += load.Miles + load.DeadheadOrigin + load.DeadheadDestination;
                        loadsByStatus.InProgressLoadsGross += load.Rate;
                        loadsByStatus.InProgressLoadsCosts += load.Rate - load.Profit;
                        break;
                    case LoadStatus.Completed:
                        loadsByStatus.CompletedLoads++;
                        loadsByStatus.CompletedLoadsMileage += load.Miles + load.DeadheadOrigin + load.DeadheadDestination;
                        loadsByStatus.CompletedLoadsGross += load.Rate;
                        loadsByStatus.CompletedLoadsCosts += load.Rate - load.Profit;
                        break;
                    default:
                        loadsByStatus.PayedLoads++;
                        loadsByStatus.PayedLoadsMileage += load.Miles + load.DeadheadOrigin + load.DeadheadDestination;
                        loadsByStatus.PayedLoadsGross += load.Rate;
                        loadsByStatus.PayedLoadsCosts += load.Rate - load.Profit;
                        break;
                }
            }

            loadsByStatus.AllLoadsMileage = loadsByStatus.AllLoadsMileage > 0 ? double.Round(loadsByStatus.AllLoadsMileage, 0) : 0;
            loadsByStatus.AllLoadsGross = loadsByStatus.AllLoadsGross > 0 ? decimal.Round(loadsByStatus.AllLoadsGross, 2) : 0;
            loadsByStatus.AllLoadsCosts = loadsByStatus.AllLoadsCosts > 0 ? decimal.Round(loadsByStatus.AllLoadsCosts, 2) : 0;

            loadsByStatus.SavedLoadsMileage = loadsByStatus.SavedLoadsMileage > 0 ? double.Round(loadsByStatus.SavedLoadsMileage, 0) : 0;
            loadsByStatus.SavedLoadsGross = loadsByStatus.SavedLoadsGross > 0 ? decimal.Round(loadsByStatus.SavedLoadsGross, 2) : 0;
            loadsByStatus.SavedLoadsCosts = loadsByStatus.SavedLoadsCosts > 0 ? decimal.Round(loadsByStatus.SavedLoadsCosts, 2) : 0;

            loadsByStatus.BookedLoadsMileage = loadsByStatus.BookedLoadsMileage > 0 ? double.Round(loadsByStatus.BookedLoadsMileage, 0) : 0;
            loadsByStatus.BookedLoadsGross = loadsByStatus.BookedLoadsGross > 0 ? decimal.Round(loadsByStatus.BookedLoadsGross, 2) : 0;
            loadsByStatus.BookedLoadsCosts = loadsByStatus.BookedLoadsCosts > 0 ? decimal.Round(loadsByStatus.BookedLoadsCosts, 2) : 0;

            loadsByStatus.InProgressLoadsMileage = loadsByStatus.InProgressLoadsMileage > 0 ? double.Round(loadsByStatus.InProgressLoadsMileage, 0) : 0;
            loadsByStatus.InProgressLoadsGross = loadsByStatus.InProgressLoadsGross > 0 ? decimal.Round(loadsByStatus.InProgressLoadsGross, 2) : 0;
            loadsByStatus.InProgressLoadsCosts = loadsByStatus.InProgressLoadsCosts > 0 ? decimal.Round(loadsByStatus.InProgressLoadsCosts, 2) : 0;

            loadsByStatus.CompletedLoadsMileage = loadsByStatus.CompletedLoadsMileage > 0 ? double.Round(loadsByStatus.CompletedLoadsMileage, 0) : 0;
            loadsByStatus.CompletedLoadsGross = loadsByStatus.CompletedLoadsGross > 0 ? decimal.Round(loadsByStatus.CompletedLoadsGross, 2) : 0;
            loadsByStatus.CompletedLoadsCosts = loadsByStatus.CompletedLoadsCosts > 0 ? decimal.Round(loadsByStatus.CompletedLoadsCosts, 2) : 0;

            loadsByStatus.PayedLoadsMileage = loadsByStatus.PayedLoadsMileage > 0 ? double.Round(loadsByStatus.PayedLoadsMileage, 0) : 0;
            loadsByStatus.PayedLoadsGross = loadsByStatus.PayedLoadsGross > 0 ? decimal.Round(loadsByStatus.PayedLoadsGross, 2) : 0;
            loadsByStatus.PayedLoadsCosts = loadsByStatus.PayedLoadsCosts > 0 ? decimal.Round(loadsByStatus.PayedLoadsCosts, 2) : 0;

            return loadsByStatus;
        }

        public async Task<WeekResults> GetWeekResultsAsync(string userId)
        {
            // Get all loads by userId
            var result = new SearchParams<LoadDto>()
            {
                CurrentPage = 1,
                PageSize = 10000,
                SearchCriteria = string.Empty,
                SortField = string.Empty,
                Order = OrderType.Ascending,
                ItemList = []
            };

            // get user's trucks amount
            SqlParameter[] parameters =
                {
                    new("@id", SqlDbType.NVarChar) { Value = userId },
                    new("@intResult", SqlDbType.Int) { Direction = ParameterDirection.Output }
                };
            var trucks = await Repository.GetIntValue("EXEC sp_getUserTrucksNumber @id, @intResult OUTPUT", parameters);
            if (trucks == 0) trucks = 1; // not possible, but anyway...

            var weekResults = new WeekResults()
            {
                StartDate = DateTime.UtcNow.AddDays(-7),
                FinishDate = DateTime.UtcNow,
                TotalMiles = 0,
                MilesPerTruck = 0,
                TotalRate = 0,
                AverageRate = 0,
                TotalProfit = 0,
                TotalCosts = 0
            };

            var filters = new List<Expression<Func<Load, bool>>>
            {
                l => l.UserId == userId,
                l => l.PickUp > weekResults.StartDate,
                l => l.PickUp < weekResults.FinishDate
            };

            await Search(result, filters, null, null);

            foreach (var load in result.ItemList)
            {
                if (load.LoadStatus != LoadStatus.Saved) // Exclude load's status 'Saved'
                {
                    weekResults.TotalMiles += load.DeadheadOrigin + load.Miles + load.DeadheadDestination;
                    weekResults.TotalRate += load.Rate;
                    weekResults.TotalProfit += load.Profit;
                }
            }

            weekResults.TotalMiles = double.Round(weekResults.TotalMiles, 0);
            weekResults.TotalRate = decimal.Round(weekResults.TotalRate, 2);
            weekResults.TotalProfit = decimal.Round(weekResults.TotalProfit, 2);

            weekResults.AverageRate = weekResults.TotalMiles > 0 ? decimal.Round(weekResults.TotalRate / (decimal)weekResults.TotalMiles, 2) : 0;
            weekResults.TotalCosts = decimal.Round(weekResults.TotalRate - weekResults.TotalProfit, 2);
            weekResults.MilesPerTruck = double.Round(weekResults.TotalMiles / trucks, 0);

            return weekResults;
        }
    }
}
