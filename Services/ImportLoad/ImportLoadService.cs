using AutoMapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq.Expressions;
using TruckDispatcherApi.Data;
using TruckDispatcherApi.Library;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class ImportLoadService(
        IMapper mapper,
        IRepository<ImportLoad> repository,
        IServiceResult<ImportLoad> serviceResult) : AppBaseService<ImportLoad, ImportLoadDto>(mapper, repository, serviceResult), IImportLoadService
    {
        public async Task ChangePickupAndDeliveryDatesForTestsAsync(int days)
        {
            ServiceResult = await Repository.GetAsync(int.MaxValue, 1, null, null, null);

            foreach (var importLoad in ServiceResult.Items)
            {
                importLoad.PickUp = importLoad.PickUp.AddDays(days);
                importLoad.Delivery = importLoad.Delivery.AddDays(days);

                await Repository.SaveAsync(importLoad);
            }
        }

        public async Task DeleteLegacy()
        {
            SqlParameter[] parameters = [new("@datetimenow", SqlDbType.DateTime2) { Value = DateTime.UtcNow }];

            await Repository.ExecuteSQLQueryAsync("EXEC sp_deleteLegacyImportLoads @datetimenow", parameters);
        }

        public async Task<ISearchParams<ImportLoadDto>> GetAsync(BrokerLoadSearchParams<ImportLoadDto> searchParams)
        {
            // filtering
            var filters = new List<Expression<Func<ImportLoad, bool>>>
            {
                l => l.ShipperId == searchParams.UserId,
                l => l.Origin.Contains(searchParams.SearchCriteria) || l.Destination.Contains(searchParams.SearchCriteria) ||
                    l.ReferenceId.Contains(searchParams.SearchCriteria)
            };
            if (searchParams.Equipment != Equipment.All) filters.Add(l => l.Equipment == searchParams.Equipment);

            // sorting by Origin, Destination, PickUp, Delivery, Miles, Rate or Rate Per Mile
            Func<IQueryable<ImportLoad>, IOrderedQueryable<ImportLoad>>? orderBy = null;
            if (searchParams.Order != OrderType.None)
            {
                orderBy = searchParams.SortField switch
                {
                    "Origin" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.Origin) : q => q.OrderByDescending(l => l.Origin),
                    "Destination" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.Destination) : q => q.OrderByDescending(l => l.Destination),
                    "PickUp" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.PickUp) : q => q.OrderByDescending(l => l.PickUp),
                    "Delivery" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.Delivery) : q => q.OrderByDescending(l => l.Delivery),
                    "Miles" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.Miles) : q => q.OrderByDescending(l => l.Miles),
                    "Rate" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.Rate) : q => q.OrderByDescending(l => l.Rate),
                    _ => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(l => l.RatePerMile) : q => q.OrderByDescending(l => l.RatePerMile),
                };
            }

            var result = new BrokerLoadSearchParams<ImportLoadDto>()
            {
                PageSize = searchParams.PageSize,
                CurrentPage = searchParams.CurrentPage,
                SearchCriteria = searchParams.SearchCriteria,
                UserId = searchParams.UserId,
                SortField = searchParams.SortField,
                Order = searchParams.Order,
                IncludeNavProperties = searchParams.IncludeNavProperties,
                ItemList = [],
                Equipment = searchParams.Equipment
            };

            await Search(result, filters, null, orderBy);

            return result;
        }

        public async Task<List<ImportLoadDto>> GetAsync(DateTime startDateTime, DateTime endDateTime, Equipment equipment)
        {
            // filtering
            var filters = new List<Expression<Func<ImportLoad, bool>>>
            {
                l => l.PickUp >= startDateTime,
                l => l.PickUp <= endDateTime,
                l => l.Equipment == equipment
            };

            Func<IQueryable<ImportLoad>, IOrderedQueryable<ImportLoad>>? orderBy = q => q.OrderBy(il => il.PickUp);

            // no included entities for ImportLoads

            var result = new SearchParams<ImportLoadDto>()
            {
                CurrentPage = 1,
                PageSize = int.MaxValue,
                SearchCriteria = string.Empty,
                SortField = "PickUp", // 
                Order = OrderType.Ascending,
                ItemList = []
            };

            await Search(result, filters, null, orderBy);

            return result.ItemList.ToList();
        }

        public async Task<AverageRates> GetAverageRatesAsync()
        {
            var startDateTime = DateTime.UtcNow;
            var finalDateTime = DateTime.UtcNow.AddHours(24);

            // filtering
            var filters = new List<Expression<Func<ImportLoad, bool>>>
            {
                l => l.PickUp >= startDateTime,
                l => l.PickUp <= finalDateTime,
            };

            var result = new SearchParams<ImportLoadDto>()
            {
                CurrentPage = 1,
                PageSize = 10000,
                SearchCriteria = string.Empty,
                SortField = string.Empty,
                Order = OrderType.Ascending,
                ItemList = []
            };

            await Search(result, filters, null, null);

            double sumAllMiles = 0f;
            decimal sumAllRates = 0m;
            double sumFlatbedMiles = 0f;
            decimal sumFlatbedRates = 0m;
            double sumReeferMiles = 0f;
            decimal sumReeferRates = 0m;
            double sumVanMiles = 0f;
            decimal sumVanRates = 0m;

            foreach (var load in result.ItemList)
            {
                sumAllMiles += load.Miles;
                sumAllRates += load.Rate;
                if (load.Equipment == Equipment.Flatbed)
                {
                    sumFlatbedMiles += load.Miles;
                    sumFlatbedRates += load.Rate;
                    continue;
                };
                if (load.Equipment == Equipment.Reefer)
                {
                    sumReeferMiles += load.Miles;
                    sumReeferRates += load.Rate;
                    continue;
                };
                if (load.Equipment == Equipment.Van)
                {
                    sumVanMiles += load.Miles;
                    sumVanRates += load.Rate;
                    continue;
                };
            }

            var averageRates = new AverageRates()
            {
                DateTime = startDateTime,
                All = sumAllMiles > 0 ? decimal.Round(sumAllRates / (decimal)sumAllMiles, 2) : 0,
                FlatbedRate = sumFlatbedMiles > 0 ? decimal.Round(sumFlatbedRates / (decimal)sumFlatbedMiles, 2) : 0,
                ReeferRate = sumReeferMiles > 0 ? decimal.Round(sumReeferRates / (decimal)sumReeferMiles, 2) : 0,
                VanRate = sumVanMiles > 0 ? decimal.Round(sumVanRates / (decimal)sumVanMiles, 2) : 0
            };

            return averageRates;
        }

        public async Task<ImportLoad?> IsImportLoadExistsAsync(ImportLoadDto importLoadDto)
        {
            Expression<Func<ImportLoad, bool>> searchQuery = il => il.ReferenceId.ToLower() == importLoadDto.ReferenceId.ToLower() &&
                il.Equipment == importLoadDto.Equipment;
            var importLoad = await Repository.GetAsync(searchQuery, []);

            return Mapper.Map<ImportLoad>(importLoad);
        }

        public async Task<ISearchParams<ImportLoadDto>> SearchAsync(ImportLoadSearchParams<ImportLoadDto> searchParams)
        {
            // filtering
            double originLatitudeLambda = CalculateLatitudeLambda(searchParams.Origin.Latitude, searchParams.Deadhead);

            var filters = new List<Expression<Func<ImportLoad, bool>>>
            {
                l => l.PickUp >= searchParams.PickupStartDate,
                l => l.Miles >= searchParams.MilesMin,
                l => l.Miles <= searchParams.MilesMax,
                l => l.OriginLatitude > searchParams.Origin.Latitude - originLatitudeLambda,
                l => l.OriginLatitude < searchParams.Origin.Latitude + originLatitudeLambda,
                l => l.OriginLongitude > searchParams.Origin.Longitude - 0.014457f * searchParams.Deadhead,
                l => l.OriginLongitude < searchParams.Origin.Longitude + 0.014457f * searchParams.Deadhead
            };
            if (searchParams.Destination != null)
            {
                double destinationLatitudeLambda = CalculateLatitudeLambda(searchParams.Destination.Latitude, searchParams.Deadhead);
                filters.Add(l => l.DestinationLatitude > searchParams.Destination.Latitude - destinationLatitudeLambda);
                filters.Add(l => l.DestinationLatitude < searchParams.Destination.Latitude + destinationLatitudeLambda);
                filters.Add(l => l.DestinationLongitude > searchParams.Destination.Longitude - 0.014457f * searchParams.Deadhead);
                filters.Add(l => l.DestinationLongitude < searchParams.Destination.Longitude + 0.014457f * searchParams.Deadhead);
            }
            // Equipment filtering includes Van type also in case of choosing Reefer-type
            if (searchParams.Truck.Equipment == Equipment.Reefer)
            {
                filters.Add(l => l.Equipment == Equipment.Reefer || l.Equipment == Equipment.Van);
            }
            else
            {
                filters.Add(l => l.Equipment == searchParams.Truck.Equipment);
            }

            Func<IQueryable<ImportLoad>, IOrderedQueryable<ImportLoad>>? orderBy = null;
            // Will sort after computing Profit/PPM/RPM in controller

            var result = new SearchParams<ImportLoadDto>()
            {
                CurrentPage = 1,
                PageSize = int.MaxValue,// Will take page size amount after sorting in the controller
                SearchCriteria = string.Empty,
                SortField = searchParams.SortField,
                Order = searchParams.Order,
                ItemList = []
            };

            await Search(result, filters, null, orderBy);

            return result;
        }

        private double CalculateLatitudeLambda(double pointLatitude, double radius)
        {
            double latitudeLambda = 0;

            latitudeLambda = pointLatitude switch
            {
                < 25 => 0.016073f * radius,
                >= 25 and < 30 => 0.016873f * radius,
                >= 30 and < 35 => 0.017897f * radius,
                >= 35 and < 40 => 0.019204f * radius,
                >= 40 and < 45 => 0.02088f * radius,
                >= 45 and < 50 => 0.023045f * radius,
                // >=50
                _ => 0.02467f * radius,
            };
            return latitudeLambda;
        }

    }
}
