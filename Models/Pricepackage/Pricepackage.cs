using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TruckDispatcherApi.Models
{
    public class Pricepackage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [Column(TypeName = "nvarchar(20)")]
        public required string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        /// <summary>
        /// Number of months: 1, 3, or 12.
        /// </summary>
        [Required(ErrorMessage = "Number of months is required.")]
        public int Period { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public required string Posibilities { get; set; }
    }
}
