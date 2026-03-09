using ClosedXML.Excel;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Development_Demand_Forecasting.Services
{
    public static class ExcelExportService
    {
        public static void ExportForecast(List<ForecastResult> forecastList)
        {
            if (forecastList == null || !forecastList.Any())
            {
                MessageBox.Show("No data to export.");
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = "ForecastResults.xlsx"
            };

            if (dialog.ShowDialog() != true)
                return;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Forecast");

                worksheet.Cell(1, 1).Value = "Product ID";
                worksheet.Cell(1, 2).Value = "Product Name";
                worksheet.Cell(1, 3).Value = "Current Stock";
                worksheet.Cell(1, 4).Value = "Predicted Demand";
                worksheet.Cell(1, 5).Value = "Suggested Order";

                for (int i = 0; i < forecastList.Count; i++)
                {
                    var item = forecastList[i];

                    worksheet.Cell(i + 2, 1).Value = item.ProductId;
                    worksheet.Cell(i + 2, 2).Value = item.ProductName;
                    worksheet.Cell(i + 2, 3).Value = item.CurrentStock;
                    worksheet.Cell(i + 2, 4).Value = item.AIPredictedDemand;
                    worksheet.Cell(i + 2, 5).Value = item.SuggestedOrderQty;
                }

                worksheet.Columns().AdjustToContents();

                var table = worksheet.Range(1, 1, forecastList.Count + 1, 5).CreateTable();
                table.Theme = XLTableTheme.TableStyleMedium2;

                workbook.SaveAs(dialog.FileName);
            }

            MessageBox.Show("Excel exported successfully.");
        }
    }
}