using System;
using System.Collections.Generic;
using System.Text;

namespace Development_Demand_Forecasting.Services
{
    public static class ForecastDataService
    {
        public static List<ForecastResult> LatestForecast { get; set; } = new List<ForecastResult>();
    }
}
