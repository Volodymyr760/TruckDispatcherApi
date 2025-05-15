using AutoMapper;
using System.Linq.Expressions;
using TruckDispatcherApi.Data;
using TruckDispatcherApi.Library;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class DriverService(IMapper mapper,
        IRepository<Driver> repository,
        IServiceResult<Driver> serviceResult) : AppBaseService<Driver, DriverDto>(mapper, repository, serviceResult), IDriverService
    {
        public async Task<ISearchParams<DriverDto>> GetAsync(ISearchParams<DriverDto> searchParams)
        {
            // filtering
            var filters = new List<Expression<Func<Driver, bool>>>();
            if (!string.IsNullOrEmpty(searchParams.SearchCriteria))
                filters.Add(d => d.FirstName.Contains(searchParams.SearchCriteria) || d.LastName.Contains(searchParams.SearchCriteria));
            filters.Add(d => d.UserId == searchParams.UserId);

            // Include truck for drivers
            List<Expression<Func<Driver, object>>> navProperties = searchParams.IncludeNavProperties ? [d => d.Truck!] : [];

            // sorting by First Name, Last Name, Phone or Email
            Func<IQueryable<Driver>, IOrderedQueryable<Driver>>? orderBy = null;
            if (searchParams.Order != OrderType.None)
            {
                orderBy = searchParams.SortField switch
                {
                    "Last Name" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(d => d.LastName) : q => q.OrderByDescending(d => d.LastName),
                    "Phone" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(d => d.Phone) : q => q.OrderByDescending(d => d.Phone),
                    "Email" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(d => d.Email) : q => q.OrderByDescending(d => d.Email),
                    _ => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(d => d.FirstName) : q => q.OrderByDescending(d => d.FirstName),
                };
            }

            await Search(searchParams, filters, navProperties, orderBy);

            return searchParams;
        }

        new public async Task<DriverDto?> GetAsync(string id)
        {
            Expression<Func<Driver, bool>> searchQuery = d => d.Id == id;
            List<Expression<Func<Driver, object>>> navProperties = [d => d.Truck!];

            return Mapper.Map<DriverDto>(await Repository.GetAsync(searchQuery, navProperties));
        }

        public async Task<IEnumerable<SearchDriverDto>> GetSearchDriversAsync(string name, string userId)
        {
            // filtering
            var filters = new List<Expression<Func<Driver, bool>>>();
            if (!string.IsNullOrEmpty(name))
                filters.Add(d => d.FirstName.Contains(name) || d.LastName.Contains(name));
            filters.Add(d => d.UserId == userId);

            // sorting
            Func<IQueryable<Driver>, IOrderedQueryable<Driver>>? orderBy = q => q.OrderBy(c => c.FirstName).ThenBy(c => c.LastName);

            var searchDrivers = await Repository.GetAsync(int.MaxValue, 1, filters, null, orderBy);

            return Mapper.Map<IEnumerable<SearchDriverDto>>(searchDrivers.Items);
        }

        public async Task RemoveAssignedTruckAsync(string driverId)
        {
            Expression<Func<Driver, bool>> searchQuery = d => d.Id == driverId;
            var driver = await Repository.GetAsync(searchQuery, []);
            if (driver != null)
            {
                driver.TruckId = null;
                await Repository.SaveAsync(driver);
            }
        }
    }
}
