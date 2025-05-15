using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Services
{
    public interface IUserSearchParams : ISearchParams<UserDto>
    {
        AccountStatus AccountStatus { get; set; }
    }
}
