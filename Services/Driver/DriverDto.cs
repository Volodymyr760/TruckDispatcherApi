using System.ComponentModel.DataAnnotations;

namespace TruckDispatcherApi.Services
{
    public class DriverDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "User id is required."), StringLength(450)]
        public required string UserId { get; set; }

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

        /// <summary>
        /// Assigned to truck Id (if any). Nonrequired link to Truck entity
        /// </summary>
        [StringLength(450)]
        public string? TruckId { get; set; }

        /// <summary>
        /// Assigned to truck (if any). Nonrequired nav link to Truck entity
        /// </summary>
        public TruckDto? Truck { get; set; }

        [StringLength(450)]
        public string? Notes { get; set; }
    }
}
