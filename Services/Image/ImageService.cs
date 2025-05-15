using AutoMapper;
using System.Linq.Expressions;
using TruckDispatcherApi.Data;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class ImageService(IMapper mapper, IRepository<Image> repository, IServiceResult<Image> serviceResult,
        IWebHostEnvironment appEnv, IConfiguration configuration) :
        AppBaseService<Image, ImageDto>(mapper, repository, serviceResult), IImageService
    {
        private readonly IWebHostEnvironment appEnv = appEnv;
        private readonly IConfiguration config = configuration;

        public async Task<ImageDto> CreateAsync(IFormFile formFile, decimal? latitude, decimal? longitude, string userId)
        {
            var extention = formFile.FileName.Substring(formFile.FileName.LastIndexOf("."));
            Guid g = Guid.NewGuid();
            var uniqueFileName = g.ToString().Replace("-", string.Empty) + extention;
            string path = appEnv.EnvironmentName == "Development" ? config["RootPath:DevContent"] + uniqueFileName :
                config["RootPath:ProdContent"] + uniqueFileName;

            ImageDto imageDto = new()
            {
                FileName = uniqueFileName,
                FullPath = appEnv.EnvironmentName == "Development" ? config["RootPath:DevWeb"] + uniqueFileName :
                                config["RootPath:ProdWeb"] + uniqueFileName,
                Extension = extention,
                Mime = string.IsNullOrEmpty(formFile.ContentType) ? "Unknown" : formFile.ContentType,
                FileSize = formFile.Length,
                Latitude = latitude,
                Longitude = longitude,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            var image = Mapper.Map<Image>(imageDto);
            using (var fileStream = new FileStream(path, FileMode.Create)) { await formFile.CopyToAsync(fileStream); }

            return Mapper.Map<ImageDto>(await Repository.CreateAsync(image));
        }

        new public async Task DeleteAsync(string fileName)
        {
            var image = await GetByFileNameAsync(fileName);
            var tasks = new List<Task>();
            if (image != null) tasks.Add(Task.Run(async () => { await Repository.DeleteAsync(image.Id); }));
            FileInfo file = new FileInfo(appEnv.EnvironmentName == "Development" ? config["RootPath:DevContent"] + fileName :
                config["RootPath:ProdContent"] + fileName);
            if (file.Exists) tasks.Add(Task.Run(file.Delete));
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                throw new DeleteEntityFailedException(image.Id, typeof(Image), ex.InnerException);
            }
        }

        public async Task<ISearchParams<ImageDto>> GetAsync(ISearchParams<ImageDto> searchParams)
        {
            // filtering only by user's Id
            var filters = new List<Expression<Func<Image, bool>>> { d => d.UserId == searchParams.UserId };

            // no navigation properties
            // no sorting

            await Search(searchParams, filters, null, null);

            return searchParams;
        }

        public async Task<ImageDto> GetByFileNameAsync(string fileName)
        {
            Expression<Func<Image, bool>> searchQuery = i => i.FileName == fileName;

            return Mapper.Map<ImageDto>(await Repository.GetAsync(searchQuery, []));
        }
    }
}
