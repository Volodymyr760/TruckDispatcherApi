using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TruckDispatcherApi.Services
{
    public class ImageDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "User id is required."), StringLength(450)]
        public required string UserId { get; set; }

        [Required(ErrorMessage = "File Name is required."), StringLength(50)]
        public required string FileName { get; set; }

        [Required(ErrorMessage = "Full Path is required."), StringLength(450)]
        public required string FullPath { get; set; }

        [Required(ErrorMessage = "Extension is required."), StringLength(10)]
        public required string Extension { get; set; }

        [Required(ErrorMessage = "Mime type is required."), StringLength(450)]
        public required string Mime { get; set; }

        [Required]
        public long FileSize { get; set; }

        [Column(TypeName = "decimal(9, 7)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(10, 7)")]
        public decimal? Longitude { get; set; }

        [Column(TypeName = "datetime2(0)")]
        public DateTime CreatedAt { get; set; }
    }
}
