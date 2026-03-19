using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Development_Demand_Forecasting.Services
{
    public static class ExcelExportService
    {
        public static void ExportData(IEnumerable<object> data, string defaultFileName = "ExportData.xlsx")
        {
            if (data == null || !data.Any())
            {
                MessageBox.Show("No data to export.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = defaultFileName
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var itemType = data.First().GetType();
                    string sheetName = itemType.Name.Length > 31 ? itemType.Name.Substring(0, 31) : itemType.Name;
                    var worksheet = workbook.Worksheets.Add(sheetName);

                    var properties = itemType.GetProperties()
                        .Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string))
                        .ToList();

                    for (int i = 0; i < properties.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = properties[i].Name;
                    }

                    int rowIndex = 2;
                    foreach (var item in data)
                    {
                        for (int colIndex = 0; colIndex < properties.Count; colIndex++)
                        {
                            var val = properties[colIndex].GetValue(item);
                            if (val != null)
                            {
                                if (val is DateTime dt)
                                    worksheet.Cell(rowIndex, colIndex + 1).Value = dt.ToString("yyyy-MM-dd");
                                else if (val is int || val is decimal || val is double || val is float || val is short || val is long)
                                    worksheet.Cell(rowIndex, colIndex + 1).Value = Convert.ToDouble(val);
                                else
                                    worksheet.Cell(rowIndex, colIndex + 1).Value = val.ToString();
                            }
                        }
                        rowIndex++;
                    }

                    worksheet.Columns().AdjustToContents();

                    var table = worksheet.Range(1, 1, rowIndex - 1, properties.Count).CreateTable();
                    table.Theme = XLTableTheme.TableStyleMedium2;

                    workbook.SaveAs(dialog.FileName);
                }

                MessageBox.Show("Data exported successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting data:\n{ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void ExportForecast(List<ForecastResult> forecastList)
        {
            if (forecastList == null || !forecastList.Any())
            {
                MessageBox.Show("No data to export.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ExportData(forecastList.Cast<object>(), "ForecastResults.xlsx");
        }
    }
}