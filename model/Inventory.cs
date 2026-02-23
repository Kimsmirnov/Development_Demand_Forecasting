using System.ComponentModel.DataAnnotations;

namespace Development_Demand_Forecasting.Model
{
    public class Inventory
    {
        [Key] // Primary key for the Inventory table
        public int InventoryId { get; set; }

        // Foreign key to the Products table
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }

        public int QuantityOnHand { get; set; }

        // Navigation properties
        public Products Product { get; set; }
        public Warehouses Warehouse { get; set; }

        public Inventory() { }

        public Inventory(int inventoryId, int productId, int warehouseId, int quantityOnHand)
        {
            this.InventoryId = inventoryId;
            this.ProductId = productId;
            this.WarehouseId = warehouseId;
            this.QuantityOnHand = quantityOnHand;
        }
    }
}