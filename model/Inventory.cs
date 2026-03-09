using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Development_Demand_Forecasting.Model
{
    public class Inventory
    {
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }

        public Products Product { get; set; }
        public Warehouses Warehouse { get; set; }

        public int QuantityOnHand { get; set; }

        [NotMapped]
        public string ProductName => Product?.Name;

        [NotMapped]
        public string WarehouseName => Warehouse?.Name;
    }
}