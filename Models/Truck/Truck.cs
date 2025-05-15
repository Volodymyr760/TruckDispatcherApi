using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Models
{
    public class Truck
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string Id { get; set; }

        [Required(ErrorMessage = "Model or name is required."), StringLength(450)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "License plate is required."), StringLength(450)]
        public required string LicensePlate {  get; set; }

        [Required]
        public Equipment Equipment { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CostPerMile { get; set; }

        [StringLength(50)]
        public string? Avatar { get; set; }

        /// <summary>
        /// Assigned drivers (if any). Nonrequired nav link to Driver entity
        /// </summary>
        public List<Driver> Drivers { get; set; } = [];

        /// <summary>
        /// Assigned loads (if any). Nonrequired nav link to Load entity
        /// </summary>
        public List<Load> Loads { get; set; } = [];

        [Required(ErrorMessage = "UserId is required."), StringLength(450)]
        public required string UserId { get; set; }

        [Required]
        public required TruckStatus TruckStatus { get; set; }

        [StringLength(450)]
        public string? Notes { get; set; }
    }
}
