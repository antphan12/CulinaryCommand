using CulinaryCommand.PurchaseOrder.Entities;

namespace CulinaryCommand.PurchaseOrder.DTOs
{
    public class PurchaseOrderDTO
    {
        public int Id { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public int LocationId { get; set; }

        public string VendorName { get; set; } = string.Empty;

        public string? VendorContact { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        public string? Notes { get; set; }

        public List<PurchaseOrderLineDTO> Lines { get; set; } = new();
    }
}