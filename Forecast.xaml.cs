using Development_Demand_Forecasting.Model;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Win32;
using ClosedXML.Excel;
using Development_Demand_Forecasting.Services;

namespace Development_Demand_Forecasting
{
    public partial class Forecast : Window
    {
        private readonly Development_Demand_Forecasting.Model.AppContext context;

        public Forecast()
        {
            InitializeComponent();
            context = new Development_Demand_Forecasting.Model.AppContext();
            context.Database.EnsureCreated();

            resultsRadioButton.IsChecked = true;
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private bool isPanelOpen = true;

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            double from = isPanelOpen ? 0 : -SidePanel.ActualWidth;
            double to = isPanelOpen ? -SidePanel.ActualWidth : 0;

            // Animate SidePanel sliding
            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase
                {
                    EasingMode = EasingMode.EaseInOut
                }
            };
            SidePanelTransform.BeginAnimation(TranslateTransform.XProperty, animation);

            // Animate MainBoard expanding
            Storyboard mainBoardStoryboard = isPanelOpen
                ? (Storyboard)FindResource("ExpandMainBoardStoryboard")
                : (Storyboard)FindResource("CollapseMainBoardStoryboard");

            mainBoardStoryboard.Begin();

            isPanelOpen = !isPanelOpen;
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (currentItems == null || currentProperties == null)
                return;

            string filter = searchBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(filter))
            {
                accountsDataGrid.ItemsSource = currentItems;
                return;
            }

            var filtered = currentItems
                .Where(item => currentProperties.Any(p =>
                {
                    var value = p.GetValue(item);
                    if (value == null) return false;

                    if (value is DateTime dt)
                        return dt.ToString("yyyy-MM-dd").Contains(filter);

                    return value.ToString().ToLower().Contains(filter);
                }))
                .ToList();

            accountsDataGrid.ItemsSource = filtered;
        }

        private IEnumerable<object> currentItems;
        private System.Reflection.PropertyInfo[] currentProperties;

        private async void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            var item = button.DataContext;

            var result = MessageBox.Show(
                "Delete this record?\nAll related data will also be removed.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                var updatedList = currentItems.ToList();
                if (updatedList.Contains(item))
                {
                    updatedList.Remove(item);
                    currentItems = updatedList;
                }
                searchBox_TextChanged(searchBox, null);

                MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbUpdateException)
            {
                context.Entry(item).Reload();

                MessageBox.Show(
                    "Cannot delete this record.\nIt is used in other data.",
                    "Delete Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unexpected error:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void searchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            placeholderTextBlock.Visibility = Visibility.Collapsed;
        }

        private void searchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(searchBox.Text))
                placeholderTextBlock.Visibility = Visibility.Visible;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    double percentHorizontal = e.GetPosition(this).X / this.ActualWidth;
                    double percentVertical = e.GetPosition(this).Y / this.ActualHeight;

                    this.WindowState = WindowState.Normal;

                    this.Left = e.GetPosition(null).X - (this.ActualWidth * percentHorizontal);
                    this.Top = e.GetPosition(null).Y - (this.ActualHeight * percentVertical);
                }

                this.DragMove();
            }
        }

        private List<ForecastResult> ParseForecastJson(string json)
        {
            try
            {
                var parsed = JsonSerializer.Deserialize<List<ForecastResultRaw>>(json);

                var results = new List<ForecastResult>();

                foreach (var item in parsed)
                {
                    var currentStock = context.Inventory
                        .FirstOrDefault(i => i.ProductId == item.ProductId)?
                        .QuantityOnHand ?? 0;

                    results.Add(new ForecastResult
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        CurrentStock = currentStock,
                        AIPredictedDemand = item.forecast,
                        SuggestedOrderQty = item.recommended_order
                    });
                }

                return results;
            }
            catch
            {
                MessageBox.Show("AI returned invalid JSON format.");
                return new List<ForecastResult>();
            }
        }
        private void SetForecastTable(List<ForecastResult> items)
        {
            accountsDataGrid.ItemsSource = items;

            currentItems = items.Cast<object>().ToList();
            currentProperties = typeof(ForecastResult).GetProperties();
        }

        private void accountsDataGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            var grid = sender as DataGrid;

            var actionColumn = grid.Columns.FirstOrDefault(c => c.Header.ToString() == "Actions");

            if (actionColumn != null)
            {
                actionColumn.DisplayIndex = grid.Columns.Count - 1;
            }
        }

        private void accountsDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "ProductId")
            {
                e.Column.Visibility = Visibility.Collapsed;
                return;
            }

            if (e.PropertyType == typeof(DateTime))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "yyyy-MM-dd";
            }
        }

        private void Results_Checked(object sender, RoutedEventArgs e)
        {
         
        }

        private async void Forcast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn)
                {
                    if (btn.Content is StackPanel sp)
                    {
                        foreach (var child in sp.Children)
                        {
                            if (child is TextBlock tb)
                            {
                                tb.Text = "In Progress";
                                break;
                            }
                        }
                    }
                }

                var client = new Client(apiKey: "AIzaSyAWU3Nyn8OmYKyovQx2BHUAxapV-RBuuHc");

                var salesData = context.SalesHistory
                    .ToList();

                var products = context.Products
                    .ToList();

                var inventory = context.Inventory
                    .ToList();

                var productDataList = salesData
                    .GroupBy(s => s.ProductId)
                    .Select(g =>
                    {
                        var productName = products.FirstOrDefault(p => p.ProductId == g.Key)?.Name ?? "Unknown";

                        var monthlySales = g
                            .GroupBy(s => new DateTime(s.Date.Year, s.Date.Month, 1))
                            .Select(mg => mg.Sum(x => x.Quantity))
                            .ToList();

                        var currentStock = inventory.FirstOrDefault(i => i.ProductId == g.Key)?.QuantityOnHand ?? 0;

                        return $"Data for Product {g.Key} ({productName}):\nMonthly sales: [{string.Join(",", monthlySales)}]\nCurrent stock: {currentStock}";
                    })
                    .ToList();

                string dataString = string.Join("\n\n", productDataList);

                string prompt = $@"
                    Forecast next month's demand for each product below.

                    Return ONLY valid JSON array.
                    Do not add explanation text.

                    {dataString}

                    Example output:
                    [
                      {{
                        ""ProductId"": 1,
                        ""ProductName"": ""Product A"",
                        ""forecast"": 120,
                        ""recommended_order"": 50
                      }}
                    ]";

                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-3-flash-preview",
                    contents: prompt
                );

                string result = response.Candidates[0].Content.Parts[0].Text;

                var forecastList = ParseForecastJson(result);
                ForecastDataService.LatestForecast = forecastList;
                SetForecastTable(forecastList);

                if (sender is Button btnReset)
                {
                    if (btnReset.Content is StackPanel spReset)
                    {
                        foreach (var child in spReset.Children)
                        {
                            if (child is TextBlock tb)
                            {
                                tb.Text = "Forcast";
                                break;
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Forecast Error", MessageBoxButton.OK, MessageBoxImage.Error);

                if (sender is Button btnReset)
                {
                    if (btnReset.Content is StackPanel spReset)
                    {
                        foreach (var child in spReset.Children)
                        {
                            if (child is TextBlock tb)
                            {
                                tb.Text = "Forcast";
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void Dashboard_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow MainWindow = new MainWindow();
            MainWindow.Show();
            this.Close();
        }

        private void export_Click(object sender, RoutedEventArgs e)
        {
            var forecastList = ForecastDataService.LatestForecast;
            ExcelExportService.ExportForecast(forecastList);
        }
    }
}
