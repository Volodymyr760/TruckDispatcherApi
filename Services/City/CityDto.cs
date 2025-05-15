using System.ComponentModel.DataAnnotations;

namespace TruckDispatcherApi.Services
{
    public class CityDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Name (50 characters) is required."), StringLength(50)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "State (2 characters) is required."), StringLength(2)]
        public required string State { get; set; }

        [Required(ErrorMessage = "FullName (50 characters) is required."), StringLength(50)]
        public required string FullName { get; set; }

        [Required]
        public required double Latitude { get; set; }

        [Required]
        public required double Longitude { get; set; }
    }
}
