// Represents a single purchase from a vendor to a specific restaurant location.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CulinaryCommand.Data.Entities;

namespace CulinaryCommand.PurchaseOrder.Entities
{
    public class PurchaseOrder
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        public int LocationId { get; set; }

        [ForeignKey(nameof(LocationId))]
        public Location Location { get; set; } = default!;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpectedDeliveryDate { get; set; }

        [Required, StringLength(256)]
        public string VendorName { get; set; } = string.Empty;
        public string? VendorContact { get; set; }

        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
        public ICollection<PurchaseOrderLine> Lines { get; set; } = new List<PurchaseOrderLine>();

        public string? Notes { get; set; }

        // for audit reasons
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // flag to prevent changing location after creation
        public bool IsLocationLocked { get; set; } = false;

    }

    public enum PurchaseOrderStatus
    {
        Draft = 0,
        Submitted = 1,
        PartiallyReceived = 2,
        Received = 3,
        Cancelled = 4
    }
}
