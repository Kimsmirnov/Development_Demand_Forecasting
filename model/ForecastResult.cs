using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Development_Demand_Forecasting
{
    public class ForecastResult
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CurrentStock { get; set; }
        public int AIPredictedDemand { get; set; }
        public int SuggestedOrderQty { get; set; }
    }
}
