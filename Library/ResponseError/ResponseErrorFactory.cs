namespace TruckDispatcherApi.Library
{
    public class ResponseErrorFactory
    {
        public static IResponseError GetBadRequestError(string title) => new BadRequestError() { Title = title };

        public static IResponseError GetNotFoundError(string title) => new NotFoundError() { Title = title };

        public static IResponseError GetServiceUnavailableError(string title) => new ServiceUnavailableError() { Title = title };

        public static IResponseError GetInternalServerError(string title) => new InternalServerError() { Title = title };
    }
}
