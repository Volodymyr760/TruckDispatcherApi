using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TruckDispatcherApi.Models
{
    public class Broker
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string Id { get; set; }

        [Required(ErrorMessage = "Parser name is required."), StringLength(450)]
        public required string ParserName { get; set; }

        [Required(ErrorMessage = "Name is required."), StringLength(450)]
        public required string Name { get; set; }

        /// <summary>
        /// ShortName is the broker name in object of ImportLoad
        /// </summary>
        [Required(ErrorMessage = "Short name is required."), StringLength(450)]
        public required string ShortName { get; set; }

        [StringLength(50)]
        public string? Logo { get; set; }

        [Required(ErrorMessage = "Email is required."), StringLength(256)]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Phone is required."), StringLength(20)]
        public required string Phone { get; set; }

        [Required(ErrorMessage = "DOT is required."), StringLength(20)]
        public required string DotNumber { get; set; }

        [Required(ErrorMessage = "MC is required."), StringLength(20)]
        public required string McNumber { get; set; }

        [StringLength(450)]
        public string? Notes { get; set; }
    }
}
