

namespace TruckDispatcherApi.Services
{
    public interface IParser
    {
        string Name { get; set; }

        string RootLink { get; set; }

        Task<List<ImportLoadDto>> ParseAsync(string jsonString);
    }
}