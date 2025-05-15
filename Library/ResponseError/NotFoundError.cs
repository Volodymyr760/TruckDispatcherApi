namespace TruckDispatcherApi.Library
{
    public class NotFoundError : IResponseError
    {
        public int Status { get; } = 404;
        public string Title { get; set; } = string.Empty;
    }
}
