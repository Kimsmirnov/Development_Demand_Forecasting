using System.ComponentModel.DataAnnotations;

namespace Development_Demand_Forecasting.Model
{
    public class Suppliers
    {
        [Key]
        public int SupplierId { get; set; }

        public string Name { get; set; }
        public string ContactInfo { get; set; }

        public ICollection<Products> Products { get; set; }
    }
}