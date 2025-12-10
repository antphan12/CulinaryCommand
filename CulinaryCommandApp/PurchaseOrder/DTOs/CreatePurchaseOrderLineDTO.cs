using System.ComponentModel.DataAnnotations;

namespace CulinaryCommand.PurchaseOrder.DTOs
{
    public class CreatePurchaseOrderLineDTO
    {
        [Required]
        public int IngredientId { get; set; }

        [Required]
        public int UnitId { get; set; }

        [Range(0.0001, double.MaxValue)]
        public decimal QuantityOrdered { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        
    }
}