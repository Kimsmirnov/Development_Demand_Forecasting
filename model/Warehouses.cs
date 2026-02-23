using System.ComponentModel.DataAnnotations;

namespace Development_Demand_Forecasting.Model
{
    public class Warehouses
    {
        [Key]
        public int WarehouseId { get; set; }

        public string Name { get; set; }
        public string Location { get; set; }

        public Warehouses() { }

        public Warehouses(int warehouseId, string name, string location)
        {
            this.WarehouseId = warehouseId;
            this.Name = name;
            this.Location = location;

        }
    }
}