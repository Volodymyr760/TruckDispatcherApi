using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Services
{
    public class AuthModel
    {
        public UserDto User { get; set; }

        public IList<string> Roles { get; set; }

        public TokenModel Tokens { get; set; }
    }
}
