using System.ComponentModel.DataAnnotations;

namespace Development_Demand_Forecasting.Model
{
    public class Products
    {
        [Key]
        public int ProductId { get; set; }

        public string Name { get; set; }
        public decimal UnitPrice { get; set; }

        public int SupplierId { get; set; }

        public Suppliers Supplier { get; set; }

        public ICollection<SalesHistory> SalesHistory { get; set; }
        public ICollection<Inventory> Inventory { get; set; }
    }
}