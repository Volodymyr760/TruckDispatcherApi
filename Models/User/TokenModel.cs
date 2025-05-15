using System.ComponentModel.DataAnnotations;

namespace TruckDispatcherApi.Models
{
    public class TokenModel
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
