using System.ComponentModel.DataAnnotations;

namespace Development_Demand_Forecasting.Model
{
    public class Products
    {
        [Key]// Primary key for the Products table
        public int ProductId { get; set; }

        public string Name { get; set; }
        public decimal UnitPrice { get; set; }

        // Foreign key to the Suppliers table
        public int SupplierId { get; set; }

        // Navigation property to the Suppliers table
        public Suppliers Supplier { get; set; }

        public Products() { }

        public Products(int productId, string name, decimal unitPrice, int supplierId)
        {
            this.ProductId = productId;
            this.Name = name;
            this.UnitPrice = unitPrice;
            this.SupplierId = supplierId;
        }
    }
}