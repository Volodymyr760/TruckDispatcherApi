using Microsoft.AspNetCore.Mvc;
using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Controllers
{
    public abstract class AppControllerBase : ControllerBase
    {
        public IResponseError responseBadRequestError = ResponseErrorFactory.GetBadRequestError("Wrong given data.");
        public IResponseError responseNotFoundError = ResponseErrorFactory.GetNotFoundError("Not found.");
        public IResponseError responseServiceUnavailableError = ResponseErrorFactory.GetServiceUnavailableError("Service unavailable.");
        public IResponseError responseInternalServerError = ResponseErrorFactory.GetServiceUnavailableError("Internal server error.");
    }
}
