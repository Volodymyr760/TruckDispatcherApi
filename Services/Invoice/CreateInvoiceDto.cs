using System.ComponentModel.DataAnnotations;

namespace TruckDispatcherApi.Services
{
    public class CreateInvoiceDto
    {
        [Required(ErrorMessage = "Invoice To is required."), StringLength(450)]
        public required string InvoiceTo { get; set; }

        [Required(ErrorMessage = "Item is required."), StringLength(450)]
        public required string Item { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string? Notes { get; set; }
    }
}
