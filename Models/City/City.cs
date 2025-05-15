using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckDispatcherApi.Models
{
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string Id { get; set; }

        [Required(ErrorMessage = "Name (50 characters) is required."), StringLength(50)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "State (2 characters) is required."), StringLength(2)]
        public required string State { get; set; }

        [Required(ErrorMessage = "FullName (50 characters) is required."), StringLength(50)]
        public required string FullName { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }
    }
}
