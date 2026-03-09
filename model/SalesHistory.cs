using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Development_Demand_Forecasting.Model
{
    public class SalesHistory
    {
        [Key]
        public int SaleId { get; set; }

        public int ProductId { get; set; }

        public Products Product { get; set; }

        public DateTime Date { get; set; }
        public int Quantity { get; set; }

        [NotMapped]
        public string ProductName => Product?.Name;
    }
}