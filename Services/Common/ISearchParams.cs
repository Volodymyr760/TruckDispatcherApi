using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Services
{
    public interface ISearchParams<TModel> where TModel : class
    {
        int PageSize { get; set; }

        int CurrentPage { get; set; }

        string SearchCriteria { get; set; }

        string? UserId { get; set; }

        string SortField { get; set; }

        OrderType Order { get; set; }

        bool IncludeNavProperties { get; set; }

        IEnumerable<TModel> ItemList { get; set; }

        int PageCount { get; set; }

        int TotalItemsCount { get; set; }
    }

    public class SearchParams<TModel> : ISearchParams<TModel> where TModel : class
    {
        public int PageSize { get; set; }

        public int CurrentPage { get; set; }

        public required string SearchCriteria { get; set; }

        public string? UserId { get; set; }

        public required string SortField { get; set; }

        public OrderType Order { get; set; }

        public bool IncludeNavProperties { get; set; }

        public IEnumerable<TModel> ItemList { get; set; } = [];

        public int PageCount { get; set; }

        public int TotalItemsCount { get; set; }
    }
}
