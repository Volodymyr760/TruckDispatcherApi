namespace TruckDispatcherApi.Services
{
    public interface IServiceResult<TModel> where TModel : class
    {
        IEnumerable<TModel> Items { get; set; }

        int TotalCount { get; set; }
    }

    public class ServiceResult<TModel> : IServiceResult<TModel> where TModel : class
    {
        public IEnumerable<TModel> Items { get; set; } = [];

        public int TotalCount { get; set; }
    }
}
