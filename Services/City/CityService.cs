using AutoMapper;
using System.Linq.Expressions;
using TruckDispatcherApi.Data;
using TruckDispatcherApi.Library;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class CityService(
        IMapper mapper,
        IRepository<City> repository,
        IServiceResult<City> serviceResult) : AppBaseService<City, CityDto>(mapper, repository, serviceResult), ICityService
    {
        const double R = 6378100 / 1000 / 1.609344; // earth radius in meters to miles 

        public async Task<ISearchParams<CityDto>> GetAsync(ISearchParams<CityDto> searchParams)
        {
            // filtering
            var filters = new List<Expression<Func<City, bool>>>();
            if (!string.IsNullOrEmpty(searchParams.SearchCriteria))
                filters.Add(c => c.FullName.Contains(searchParams.SearchCriteria));

            // sorting by FullName, Latitude or Longitude
            Func<IQueryable<City>, IOrderedQueryable<City>>? orderBy = null;
            if (searchParams.Order != OrderType.None)
            {
                orderBy = searchParams.SortField switch
                {
                    "Latitude" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(c => c.Latitude) : q => q.OrderByDescending(c => c.Latitude),
                    "Longitude" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(c => c.Longitude) : q => q.OrderByDescending(c => c.Longitude),
                    _ => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(c => c.FullName) : q => q.OrderByDescending(c => c.FullName),
                };
            }

            await Search(searchParams, filters: filters, orderBy: orderBy);

            return searchParams;
        }

        public async Task<List<CityDto>> GetDuplicatesAsync()
        {
            var query = "select Id, FullName, Latitude, Longitude from Cities order by FullName";
            var cities = await Repository.GetAsync(query, null);
            var duplicatedCities = new List<CityDto>();

            for (int i = 0; i < cities.Count - 1; i++)
            {
                if (cities[i].FullName.ToLower() == cities[i + 1].FullName.ToLower())
                {
                    duplicatedCities.Add(base.Mapper.Map<CityDto>(cities[i]));
                    duplicatedCities.Add(base.Mapper.Map<CityDto>(cities[i + 1]));
                }
            }

            return duplicatedCities;
        }

        public double CalculateDistance(CityDto origin, CityDto destination) =>
            CalculateDistance(origin.Latitude, origin.Longitude, destination.Latitude, destination.Longitude);

        public double CalculateDistance(double originLatitude, double originLongitude, double destinationLatitude, double destinationLongitude)
        {
            if (originLatitude == destinationLatitude &&
                originLongitude == destinationLongitude) return 0.01;// make sense in graph f.e. when start vertex == first load

            var dLat = ToRadians(destinationLatitude - originLatitude);
            var dLon = ToRadians(destinationLongitude - originLongitude);
            var latO = ToRadians(originLatitude);
            var latD = ToRadians(destinationLatitude);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(latO) * Math.Cos(latD);
            var c = 2 * Math.Asin(Math.Sqrt(a));
            return double.Round(R * c, 0);
        }

        private static double ToRadians(double angle) => Math.PI * angle / 180.0;

        public async Task<CityDto> GetCityByFullNameAsync(string fullName)
        {
            Expression<Func<City, bool>> searchQuery = c => c.FullName.ToLower() == fullName.ToLower();

            return Mapper.Map<CityDto>(await Repository.GetAsync(searchQuery, []));
        }

        public bool IsStateAllowed(string state)
        {
            var allStates = GetStates();
            for (int i = 0; i < allStates.Count(); i++) if (allStates[i] == state.ToUpper()) return true;

            return false;
        }

        /// <summary>
        /// Returns list of all states excluding Alaska and Havai
        /// </summary>
        /// <returns></returns>
        public List<string> GetStates()
        {
            return new List<string>()
            {
                "AL", "AR", "AZ", "CA", "CO", "CT", "DE", "FL", "GA", "IA",
                "ID", "IL", "IN", "KS", "KY", "LA", "MA", "MD", "ME", "MI",
                "MN", "MO", "MS", "MT", "NC", "ND", "NE", "NH", "NJ", "NM",
                "NV", "NY", "OH", "OK", "OR", "PA", "RI", "SC", "SD", "TN",
                "TX", "UT", "VA", "VT", "WA", "WI", "WV", "WY"
            };
        }
    }
}
