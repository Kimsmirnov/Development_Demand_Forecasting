using Development_Demand_Forecasting.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Development_Demand_Forecasting
{
    public partial class AddItem : Window
    {
        private string _entityType;
        private Development_Demand_Forecasting.Model.AppContext _context;

        public AddItem(string entityType)
        {
            InitializeComponent();
            _entityType = entityType;
            _context = new Development_Demand_Forecasting.Model.AppContext();

            SetupUI();
        }

        private void SetupUI()
        {
            TitleTextBlock.Text = $"Add New {_entityType}";

            switch (_entityType)
            {
                case "Product":
                    Field1Label.Text = "Name";
                    Field2Label.Text = "Unit Price (e.g. 19.99)";

                    Field3Label.Text = "Supplier";
                    Field3TextBox.Visibility = Visibility.Collapsed;
                    Field3ComboBox.Visibility = Visibility.Visible;
                    Field3ComboBox.ItemsSource = _context.Suppliers.ToList();
                    Field3ComboBox.DisplayMemberPath = "Name";
                    Field3ComboBox.SelectedValuePath = "SupplierId";
                    break;

                case "Supplier":
                    Field1Label.Text = "Name";
                    Field2Label.Text = "Contact Info";
                    Field3Stack.Visibility = Visibility.Collapsed;
                    break;

                case "Warehouse":
                    Field1Label.Text = "Name";
                    Field2Label.Text = "Location";
                    Field3Stack.Visibility = Visibility.Collapsed;
                    break;

                case "Inventory":
                    Field1Label.Text = "Product";
                    Field1TextBox.Visibility = Visibility.Collapsed;
                    Field1ComboBox.Visibility = Visibility.Visible;
                    Field1ComboBox.ItemsSource = _context.Products.ToList();
                    Field1ComboBox.DisplayMemberPath = "Name";
                    Field1ComboBox.SelectedValuePath = "ProductId";

                    Field2Label.Text = "Warehouse";
                    Field2TextBox.Visibility = Visibility.Collapsed;
                    Field2ComboBox.Visibility = Visibility.Visible;
                    Field2ComboBox.ItemsSource = _context.Warehouses.ToList();
                    Field2ComboBox.DisplayMemberPath = "Name";
                    Field2ComboBox.SelectedValuePath = "WarehouseId";

                    Field3Label.Text = "Quantity On Hand";
                    break;

                case "SalesHistory":
                    Field1Label.Text = "Product";
                    Field1TextBox.Visibility = Visibility.Collapsed;
                    Field1ComboBox.Visibility = Visibility.Visible;
                    Field1ComboBox.ItemsSource = _context.Products.ToList();
                    Field1ComboBox.DisplayMemberPath = "Name";
                    Field1ComboBox.SelectedValuePath = "ProductId";

                    Field2Label.Text = "Date (yyyy-MM-dd)";
                    Field2TextBox.Text = DateTime.Now.ToString("yyyy-MM-dd");

                    Field3Label.Text = "Quantity";
                    break;
            }
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (_entityType)
                {
                    case "Product":
                        if (Field3ComboBox.SelectedValue == null) throw new Exception("Please select a Supplier.");

                        string priceText = Field2TextBox.Text.Replace(",", ".");

                        if (!decimal.TryParse(priceText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal price) || price < 0)
                        {
                            throw new Exception("Please enter a valid positive price (e.g., 19.99).");
                        }

                        price = Math.Round(price, 2);

                        var product = new Products
                        {
                            Name = Field1TextBox.Text,
                            UnitPrice = price,
                            SupplierId = (int)Field3ComboBox.SelectedValue
                        };
                        _context.Products.Add(product);
                        break;

                    case "Supplier":
                        if (string.IsNullOrWhiteSpace(Field1TextBox.Text)) throw new Exception("Supplier Name cannot be empty.");

                        var supplier = new Suppliers
                        {
                            Name = Field1TextBox.Text,
                            ContactInfo = Field2TextBox.Text
                        };
                        _context.Suppliers.Add(supplier);
                        break;

                    case "Warehouse":
                        if (string.IsNullOrWhiteSpace(Field1TextBox.Text)) throw new Exception("Warehouse Name cannot be empty.");

                        var warehouse = new Warehouses
                        {
                            Name = Field1TextBox.Text,
                            Location = Field2TextBox.Text
                        };
                        _context.Warehouses.Add(warehouse);
                        break;

                    case "Inventory":
                        if (Field1ComboBox.SelectedValue == null) throw new Exception("Please select a Product.");
                        if (Field2ComboBox.SelectedValue == null) throw new Exception("Please select a Warehouse.");

                        if (!int.TryParse(Field3TextBox.Text, out int qty) || qty < 0)
                        {
                            throw new Exception("Please enter a valid positive whole number for Quantity On Hand.");
                        }

                        var inv = new Inventory
                        {
                            ProductId = (int)Field1ComboBox.SelectedValue,
                            WarehouseId = (int)Field2ComboBox.SelectedValue,
                            QuantityOnHand = qty
                        };
                        _context.Inventory.Add(inv);
                        break;

                    case "SalesHistory":
                        if (Field1ComboBox.SelectedValue == null) throw new Exception("Please select a Product.");

                        if (!DateTime.TryParse(Field2TextBox.Text, out DateTime date))
                        {
                            throw new Exception("Please enter a valid date in the format yyyy-MM-dd.");
                        }

                        if (!int.TryParse(Field3TextBox.Text, out int saleQty) || saleQty <= 0)
                        {
                            throw new Exception("Please enter a valid whole number greater than 0 for Quantity.");
                        }

                        var sale = new SalesHistory
                        {
                            ProductId = (int)Field1ComboBox.SelectedValue,
                            Date = date,
                            Quantity = saleQty
                        };
                        _context.SalesHistory.Add(sale);
                        break;
                }

                await _context.SaveChangesAsync();
                MessageBox.Show($"{_entityType} saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Input Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) this.DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}