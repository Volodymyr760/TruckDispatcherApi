namespace TruckDispatcherApi.Library
{
    public class InternalServerError : IResponseError
    {
        public int Status { get; } = 500;
        public string Title { get; set; } = string.Empty;
    }
}
