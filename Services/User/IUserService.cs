using TruckDispatcherApi.Models;
using TruckDispatcherApi.Services.Common;

namespace TruckDispatcherApi.Services
{
    public interface IUserService : IBaseService<UserDto>
    {
        /// <summary>
        /// Needed after user has updated profile
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        void ClearCache(string key);

        new Task DeleteAsync(string id);

        Task<IUserSearchParams> GetAsync(IUserSearchParams searchParams);

        new Task<UserDto?> GetAsync(string id);

        /// <summary>
        /// Converts domain model User (used by EF Identity) to UserDto
        /// </summary>
        /// <param name="user"></param>
        /// <returns>UserDto</returns>
        UserDto GetApplicationUserDto(User user);

        /// <summary>
        /// Converts UserDto to domain model User (used by EF Identity)
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns>UserDto</returns>
        User GetApplicationUser(UserDto userDto);

        Task<UserDto?> GetByEmailAsync(string email);

        string? GetCashedUserRefreshToken(string token);

        Task<UserSearchSettings?> GetSearchSettingsAsync(string id);

        /// <summary>
        /// Checks if user has related drivers, needed before removing the user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>bool</returns>
        Task<bool> HasDriversAsync(string userId);

        /// <summary>
        /// Checks if user has related loads, needed before removing the user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>bool</returns>
        Task<bool> HasLoadsAsync(string userId);

        /// <summary>
        /// Checks if user has related trucks, needed before removing the user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>bool</returns>
        Task<bool> HasTrucksAsync(string userId);

        void SetCashedUserRefreshToken(string token);

        Task UpdateLastLoginDateAsync(string userId);
    }
}
