namespace TruckDispatcherApi.Library
{
    public class BadRequestError : IResponseError
    {
        public int Status { get; } = 400;
        public string Title { get; set; } = string.Empty;
    }
}
