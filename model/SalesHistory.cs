using System;
using System.ComponentModel.DataAnnotations;

namespace Development_Demand_Forecasting.Model
{
    public class SalesHistory
    {
        [Key]//primary key
        public int SaleId { get; set; }

        public int ProductId { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }

        //navigation property
        public Products Product { get; set; }

        public SalesHistory() { }

        public SalesHistory(int saleId, int productId, DateTime date, int quantity)
        {
            this.SaleId = saleId;
            this.ProductId = productId;
            this.Date = date;
            this.Quantity = quantity;
        }
    }
}