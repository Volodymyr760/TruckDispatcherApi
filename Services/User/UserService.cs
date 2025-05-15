using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Linq.Expressions;
using TruckDispatcherApi.Data;
using TruckDispatcherApi.Library;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class UserService(
        IMapper mapper,
        IRepository<User> repository,
        IServiceResult<User> serviceResult,
        IMemoryCache memoryCache) : AppBaseService<User, UserDto>(mapper, repository, serviceResult), IUserService
    {
        private readonly IMemoryCache cache = memoryCache;

        public void ClearCache(string key) => cache.Remove(key);

        public async Task<IUserSearchParams> GetAsync(IUserSearchParams searchParams)
        {
            // filtering
            var filters = new List<Expression<Func<User, bool>>>();
            if (!string.IsNullOrEmpty(searchParams.SearchCriteria)) filters.Add(u => u.FirstName.Contains(searchParams.SearchCriteria) ||
                u.LastName.Contains(searchParams.SearchCriteria));
            if (searchParams.AccountStatus != AccountStatus.None) filters.Add(u => u.AccountStatus == searchParams.AccountStatus);

            List<Expression<Func<User, object>>> navProperties = [];
            if (searchParams.IncludeNavProperties) navProperties.Add(u => u.Trucks);

            // sorting by First name, Last name, Email, LastloginDate, StartPayedPeriodDate or FinishPayedPeriodDate
            Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null;
            if (searchParams.Order != OrderType.None)
            {
                orderBy = searchParams.SortField switch
                {
                    "First name" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(u => u.FirstName) : q => q.OrderByDescending(u => u.FirstName),
                    "Last name" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(u => u.LastName) : q => q.OrderByDescending(u => u.LastName),
                    "Email" => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(u => u.Email) : q => q.OrderByDescending(u => u.Email),
                    "StartPayedPeriod" => searchParams.Order == OrderType.Ascending ?
                        q => q.OrderBy(u => u.StartPayedPeriodDate) : q => q.OrderByDescending(u => u.StartPayedPeriodDate),
                    "FinishPayedPeriod" => searchParams.Order == OrderType.Ascending ?
                        q => q.OrderBy(u => u.FinishPayedPeriodDate) : q => q.OrderByDescending(u => u.FinishPayedPeriodDate),
                    _ => searchParams.Order == OrderType.Ascending ? q => q.OrderBy(u => u.LastLoginDate) : q => q.OrderByDescending(u => u.LastLoginDate)
                };
            }

            await Search(searchParams, filters, navProperties, orderBy);

            return searchParams;
        }

        new public async Task<UserDto?> GetAsync(string id)
        {
            if (!cache.TryGetValue(id, out User? user))
            {
                Expression<Func<User, bool>> searchQuery = u => u.Id == id;
                List<Expression<Func<User, object>>> navProperties =
                    [
                        u => u.Drivers,
                        u => u.Loads,
                        u => u.Trucks,
                    ];
                var userFromDb = await Repository.GetAsync(searchQuery, navProperties);
                if (userFromDb != null)
                    cache.Set(id, userFromDb, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(20)));
            }

            return Mapper.Map<UserDto>(user);
        }

        public User GetApplicationUser(UserDto userDto) => Mapper.Map<User>(userDto);

        public UserDto GetApplicationUserDto(User user) => Mapper.Map<UserDto>(user);

        public async Task<UserDto?> GetByEmailAsync(string? email)
        {
            if (string.IsNullOrEmpty(email)) return null;
            if (!cache.TryGetValue(email, out UserDto? userDto))
            {
                Expression<Func<User, bool>> searchQuery = u => u.Email == email;
                List<Expression<Func<User, object>>> navProperties = [];
                var userFromDb = await Repository.GetAsync(searchQuery, navProperties);
                if (userFromDb != null)
                {
                    userDto = Mapper.Map<UserDto>(userFromDb);
                    cache.Set(email, userDto, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(20)));
                }
            }

            return userDto;
        }

        public string? GetCashedUserRefreshToken(string token) =>
            cache.TryGetValue(token, out string? tokenFromCash) ? tokenFromCash : null;

        public async Task<UserSearchSettings?> GetSearchSettingsAsync(string id)
        {
            UserSearchSettings? userSearchSettings = null;
            Expression<Func<User, bool>> searchQuery = u => u.Id == id;
            var user = await Repository.GetAsync(searchQuery, []);
            if (user != null)
            {
                userSearchSettings = new UserSearchSettings()
                {
                    UserId = id,
                    Deadheads = user.SearchDeadheads,
                    MilesMin = user.SearchMilesMin,
                    MilesMax = user.SearchMilesMax,
                    SortField = user.SearchSortField,
                    Sort = user.SearchSort
                };
            }

            return userSearchSettings;
        }

        public async Task<bool> IsExistAsync(string id)
        {
            SqlParameter[] parameters =
                [
                   new("@id", SqlDbType.NVarChar) { Value = id },
                   new("@boolResult", SqlDbType.Bit) {Direction = ParameterDirection.Output}
                ];

            return await Repository.GetBoolValue("EXEC sp_checkApplicationUserById @id,@boolResult OUTPUT", parameters);
        }

        public async Task<bool> HasDriversAsync(string userId)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.NVarChar) { Value = userId },
                   new SqlParameter("@boolResult", SqlDbType.Bit) {Direction = ParameterDirection.Output}
                };

            return await Repository.GetBoolValue("EXEC sp_checkUserDrivers @id, @boolResult OUTPUT", parameters);
        }

        public async Task<bool> HasLoadsAsync(string userId)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.NVarChar) { Value = userId },
                   new SqlParameter("@boolResult", SqlDbType.Bit) {Direction = ParameterDirection.Output}
                };

            return await Repository.GetBoolValue("EXEC sp_checkUserLoads @id, @boolResult OUTPUT", parameters);
        }

        public async Task<bool> HasTrucksAsync(string userId)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.NVarChar) { Value = userId },
                   new SqlParameter("@boolResult", SqlDbType.Bit) {Direction = ParameterDirection.Output}
                };

            return await Repository.GetBoolValue("EXEC sp_checkUserTrucks @id, @boolResult OUTPUT", parameters);
        }

        public void SetCashedUserRefreshToken(string token) =>
            cache.Set(token, token, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(60 * 2)));

        public async Task UpdateLastLoginDateAsync(string userId)
        {
            Expression<Func<User, bool>> searchQuery = a => a.Id == userId;
            var user = await Repository.GetAsync(searchQuery, []);
            if (user != null)
            {
                user.LastLoginDate = DateTime.UtcNow;
                await Repository.SaveAsync(user);
            }
        }
    }
}
