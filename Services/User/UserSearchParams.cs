using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Services
{
    public class UserSearchParams : IUserSearchParams
    {
        public int PageSize { get; set; }

        public int CurrentPage { get; set; }

        public required string SearchCriteria { get; set; }

        public string? UserId { get; set; }

        public required string SortField { get; set; }

        public OrderType Order { get; set; }
        public bool IncludeNavProperties { get; set; }

        public IEnumerable<UserDto> ItemList { get; set; } = [];

        public int PageCount { get; set; }

        public int TotalItemsCount { get; set; }

        public AccountStatus AccountStatus { get; set; }
    }
}
