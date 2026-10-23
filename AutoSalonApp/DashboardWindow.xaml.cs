using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutoSalonApp
{    public partial class DashboardWindow : Window
    {
        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;
Initial Catalog=AutoSalonDB;Integrated Security=True;TrustServerCertificate=True;";
        private string userRole;

        public DashboardWindow(string role)
        {      InitializeComponent();
            userRole = role;
                        if (TbUserInfo != null)
                TbUserInfo.Text = $"Вы вошли как:\n{userRole}";
                   
            if (userRole == "Admin" || userRole == "Manager")
            {
                if (BtnAddSale != null) BtnAddSale.Visibility = Visibility.Visible;
            }

            LoadData();
        }
                private void LoadData()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT s.SaleID AS [ID], s.SaleDate AS [Дата], 
                                     c.FullName AS [Клиент], b.BrandName + ' '
+ cr.ModelName AS [Автомобиль],
                                     s.TotalAmount AS [Сумма]
                                     FROM Sale s 
                                     JOIN Client c ON s.ClientID = c.ClientID
                                     JOIN Car cr ON s.CarID = cr.CarID 
                                     JOIN Brand b ON cr.BrandID = b.BrandID";

                    var adapter = new SqlDataAdapter(query, connection);
                    var table = new DataTable();
                    adapter.Fill(table);

                    if (MainDataGrid != null) MainDataGrid.ItemsSource = table.DefaultView;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        private void BtnSales_Click(object sender, RoutedEventArgs e) { LoadData(); }
            private void BtnAddSale_Click(object sender, RoutedEventArgs e)
        {
            AddSaleWindow addSale = new AddSaleWindow();
            addSale.ShowDialog();
            LoadData(); 
        }
         private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        } }}