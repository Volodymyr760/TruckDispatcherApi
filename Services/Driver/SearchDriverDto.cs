using System.ComponentModel.DataAnnotations;

namespace TruckDispatcherApi.Services
{
    public class SearchDriverDto
    {
        public required string Id { get; set; }

        [Required(ErrorMessage = "First name (1-20 characters) is required."), StringLength(20)]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last name (1-20 characters) is required."), StringLength(20)]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Phone is required."), StringLength(20)]
        public required string Phone { get; set; }

        [Required(ErrorMessage = "Email is required."), StringLength(256)]
        public required string Email { get; set; }

        [StringLength(50)]
        public string? Avatar { get; set; }
    }
}
