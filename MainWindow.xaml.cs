using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
using Development_Demand_Forecasting.Model;
using Google.GenAI;
using Google.GenAI.Types;

namespace Development_Demand_Forecasting
{

    public partial class MainWindow : Window
    {
        private readonly Development_Demand_Forecasting.Model.AppContext context;


        public MainWindow()
        {
            InitializeComponent();
            context = new Development_Demand_Forecasting.Model.AppContext();
            context.Database.EnsureCreated();

        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
            else if (e.ChangedButton == MouseButton.Left)
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

        private void Products_Checked(object sender, RoutedEventArgs e)
        {
            accountsDataGrid.ItemsSource = context.Products.ToList();
        }

        private void Suppliers_Checked(object sender, RoutedEventArgs e)
        {
            accountsDataGrid.ItemsSource = context.Suppliers.ToList();
        }

        private void Warehouses_Checked(object sender, RoutedEventArgs e)
        {
            accountsDataGrid.ItemsSource = context.Warehouses.ToList();
        }

        private void Inventory_Checked(object sender, RoutedEventArgs e)
        {
            accountsDataGrid.ItemsSource = context.Inventory.ToList();
        }

        private void Sales_Checked(object sender, RoutedEventArgs e)
        {
            accountsDataGrid.ItemsSource = context.SalesHistory.ToList();
        }

        private void Forecasts_Checked(object sender, RoutedEventArgs e)
        {
            accountsDataGrid.ItemsSource = context.Forecasts.ToList();
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

        private async void Forcast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = new Client(apiKey: "AIzaSyCViXsOHkMg0aflhk7BX44NxqNkueFQy60"); ;

                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-3-flash-preview",
                    contents: """
                    Forecast next month's demand based on the data below.
                    Return ONLY valid JSON.

                    Data:
                    Monthly sales: [120,135,150,170,165,180,210,205,190,200,240,260]
                    Current stock: 300

                    Output format:
                    {
                      "forecast": number,
                      "recommended_order": number,
                      "explanation": "text"
                    }
                    """
                );

                string result = response.Candidates[0].Content.Parts[0].Text;

                MessageBox.Show(result, "Forecast Result", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Forecast Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
