using System.ComponentModel.DataAnnotations;

namespace Development_Demand_Forecasting.Model
{
    public class Warehouses
    {
        [Key]
        public int WarehouseId { get; set; }

        public string Name { get; set; }
        public string Location { get; set; }

        public ICollection<Inventory> Inventory { get; set; }
    }
}