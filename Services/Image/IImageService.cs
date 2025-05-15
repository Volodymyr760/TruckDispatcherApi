using TruckDispatcherApi.Services.Common;

namespace TruckDispatcherApi.Services
{
    public interface IImageService : IBaseService<ImageDto>
    {
        Task<ImageDto> CreateAsync(IFormFile formFile, decimal? latitude, decimal? longitude, string userId);

        new Task DeleteAsync(string fileName);

        Task<ISearchParams<ImageDto>> GetAsync(ISearchParams<ImageDto> searchParams);

        Task<ImageDto> GetByFileNameAsync(string fileName);
    }
}
