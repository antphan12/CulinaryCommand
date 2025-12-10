using System.ComponentModel.DataAnnotations;

namespace CulinaryCommand.PurchaseOrder.DTOs
{
    public class CreatePurchaseOrderDTO
    {
        [Required]
        public int LocationId { get; set; }

        [Required, StringLength(200)]
        public string VendorName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? VendorContact { get; set; }

        
        public DateTime? ExpectedDeliveryDate { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
        
        [MinLength(1, ErrorMessage = "At least one line is required.")]
        public List<CreatePurchaseOrderLineDTO> Lines { get; set; } = new();
    }
}