namespace CulinaryCommand.PurchaseOrder.DTOs
{
    public class PurchaseOrderLineDTO
    {
        public int Id { get; set; }

        public int IngredientId { get; set; }

        public int UnitId { get; set; }

        public decimal QuantityOrdered { get; set; }

        public decimal QuantityReceived { get; set; }

        public decimal UnitPrice { get; set; }
    }
}