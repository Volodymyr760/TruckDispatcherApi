using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TruckDispatcherApi.Services
{
    public class InvoiceDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "InvoiceNo is required.")]
        public int InvoiceNo { get; set; }

        [Required(ErrorMessage = "InvoiceTo is required."), StringLength(450)]
        public required string InvoiceTo { get; set; }

        [Required(ErrorMessage = "Item is required."), StringLength(450)]
        public required string Item { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "Beneficiary is required."), StringLength(50)]
        public required string Beneficiary { get; set; }

        [Required(ErrorMessage = "Account is required."), StringLength(50)]
        public required string Account { get; set; }

        [Required(ErrorMessage = "Beneficiary Email is required."), StringLength(50)]
        public required string BeneficiaryEmail { get; set; }

        [Required(ErrorMessage = "Bank is required."), StringLength(50)]
        public required string Bank { get; set; }

        [Required(ErrorMessage = "Bank Address is required."), StringLength(50)]
        public required string BankAddress { get; set; }

        [Required(ErrorMessage = "Swift is required."), StringLength(50)]
        public required string Swift { get; set; }

        [Required(ErrorMessage = "Intermediary Bank is required."), StringLength(50)]
        public required string IntermediaryBank { get; set; }

        [Required(ErrorMessage = "Intermediary Swift is required."), StringLength(50)]
        public required string IntermediarySwift { get; set; }

        [Required]
        public bool IsRead { get; set; }

        [Required]
        public bool IsPaid { get; set; }

        [Column(TypeName = "datetime2(0)")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? Notes { get; set; }

        [Required(ErrorMessage = "UserId is required."), StringLength(450)]
        public required string UserId { get; set; }
    }
}
