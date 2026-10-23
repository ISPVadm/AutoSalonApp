using System;
using System.Data.SqlClient;
using System.Collections.Generic;
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
    public partial class MainWindow : Window
    {
        private string connectionString 
        = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AutoSalonDB;
Integrated Security=True;TrustServerCertificate=True;";

        public MainWindow() { InitializeComponent(); }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = TbLogin.Text.Trim();
            string password = PbPassword.Password.Trim();
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                TbError.Text = "Заполните логин и пароль!";
                return;
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Role FROM Users WHERE Login = " +
                        "@login AND Password = @password";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@password", password);
                        var role = command.ExecuteScalar()?.ToString();

                        if (role != null)
                        {
                            DashboardWindow dashboard = new DashboardWindow(role);
                            dashboard.Show();
                            this.Close();
                        }
                        else { TbError.Text = "Неверный логин или пароль!"; }
                    }
                }
            }
            catch (System.Exception ex) { MessageBox.Show("Ошибка БД: " + ex.Message); }
        }
    }
}