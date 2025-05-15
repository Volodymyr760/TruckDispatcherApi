using TruckDispatcherApi.Services.Common;

namespace TruckDispatcherApi.Services
{
    public interface ICityService : IBaseService<CityDto>
    {
        Task<ISearchParams<CityDto>> GetAsync(ISearchParams<CityDto> searchParams);

        Task<List<CityDto>> GetDuplicatesAsync();

        // Haversine formula https://stackoverflow.com/questions/41621957/a-more-efficient-haversine-function
        double CalculateDistance(double originLatitude, double originLongitude, double destinationLatitude, double destinationLongitude);

        double CalculateDistance(CityDto origin, CityDto destination);

        bool IsStateAllowed(string state);

        Task<CityDto> GetCityByFullNameAsync(string name);

        List<string> GetStates();
    }
}