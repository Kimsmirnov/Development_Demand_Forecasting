using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Development_Demand_Forecasting.Model
{
	public class Forecasts
	{
		[Key]
		public int ForecastId { get; set; }

		public int ProductId { get; set; }
		public DateTime ForecastDate { get; set; }
		public DateTime ForecastPeriodStart { get; set; }
		public DateTime ForecastPeriodEnd { get; set; }
		public decimal PredictedDemand { get; set; }
		public decimal RecommendedOrder { get; set; }
		public string Explanation { get; set; }

		public Products Product { get; set; }

        public Forecasts() { }

        public Forecasts(int forecastId, int productId, DateTime forecastDate, DateTime forecastPeriodStart, DateTime forecastPeriodEnd, decimal predictedDemand, decimal recommendedOrder, string explanation)
		{
			this.ForecastId = forecastId;
			this.ProductId = productId;
			this.ForecastDate = forecastDate;
			this.ForecastPeriodStart = forecastPeriodStart;
			this.ForecastPeriodEnd = forecastPeriodEnd;
			this.PredictedDemand = predictedDemand;
			this.RecommendedOrder = recommendedOrder;
			this.Explanation = explanation;
        }
    }
}