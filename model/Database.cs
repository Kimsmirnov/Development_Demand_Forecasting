using System.IO;
using Microsoft.Data.Sqlite;

namespace Development_Demand_Forecasting.Model
{
    public static class Database
    {
        private static readonly string DbFilePath = Path.Combine(
            System.AppContext.BaseDirectory, "Development_Demand_Forecasting.db");

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={DbFilePath}");
        }

        public static void Initialize()
        {
            
        }
    }
}
