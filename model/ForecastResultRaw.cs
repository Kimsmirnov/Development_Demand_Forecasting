using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Development_Demand_Forecasting
{
    public class ForecastResultRaw
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int forecast { get; set; }
        public int recommended_order { get; set; }
    }
}
