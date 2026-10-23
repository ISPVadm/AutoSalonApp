using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
{
    public partial class AddSaleWindow : Window
    {
        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;
Initial Catalog=AutoSalonDB;Integrated Security=True;TrustServerCertificate=True;";

        public AddSaleWindow() { InitializeComponent(); }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TbClientID.Text, out int clientId) ||
                !int.TryParse(TbCarID.Text, out int carId) ||
                !int.TryParse(TbEmpID.Text, out int empId))
            {
                MessageBox.Show("Ошибка: введите числовые ID!");
                return;
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var priceCmd = new SqlCommand("SELECT Price FROM Car WHERE CarID = @carId", connection);
                    priceCmd.Parameters.AddWithValue("@carId", carId);
                    var priceObj = priceCmd.ExecuteScalar();

                    if (priceObj == null)
                    {
                        MessageBox.Show("Автомобиль с таким ID не найден!");
                        return;
                    }

                    string insertQuery = "INSERT INTO Sale (SaleDate, TotalAmount, ClientID, EmployeeID, CarID) " +
                        "VALUES (GETDATE(), @amount, @client, @emp, @car)";
                    using (var command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@amount", Convert.ToDecimal(priceObj));
                        command.Parameters.AddWithValue("@client", clientId);
                        command.Parameters.AddWithValue("@emp", empId);
                        command.Parameters.AddWithValue("@car", carId);
                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Продажа оформлена!");
                    this.Close();
                }
            }
            catch (SqlException ex)
            {
           
                if (ex.Number == 2627) MessageBox.Show("Ошибка: Этот автомобиль уже продан!");
                else if (ex.Number == 547) MessageBox.Show("Ошибка: Неверный ID клиента или сотрудника!");
                else MessageBox.Show("Ошибка БД: " + ex.Message);
            }        }    }}