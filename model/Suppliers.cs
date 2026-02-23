using System.ComponentModel.DataAnnotations;

namespace Development_Demand_Forecasting.Model
{
    public class Suppliers
    {
        [Key]// Primary key for the Suppliers table
        public int SupplierId { get; set; }

        public string Name { get; set; }
        public string ContactInfo { get; set; }

        public Suppliers() { }

        public Suppliers(int supplierId, string name, string contactInfo)
        {
            this.SupplierId = supplierId;
            this.Name = name;
            this.ContactInfo = contactInfo;
        }
    }
}