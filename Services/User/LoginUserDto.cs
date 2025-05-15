using System.ComponentModel.DataAnnotations;

namespace TruckDispatcherApi.Services
{
    public class LoginUserDto
    {
        [Required]
        [StringLength(256, ErrorMessage = "Email is not valid")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 7)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
