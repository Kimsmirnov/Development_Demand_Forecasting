using System.ComponentModel.DataAnnotations;

namespace Development_Demand_Forecasting.Model
{
    public class Inventory
    {
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }

        public Products Product { get; set; }
        public Warehouses Warehouse { get; set; }

        public int QuantityOnHand { get; set; }
    }
}