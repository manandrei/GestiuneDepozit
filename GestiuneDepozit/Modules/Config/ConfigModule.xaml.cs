using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GestiuneDepozit.Data;
using GestiuneDepozit.Data.Models;
using GestiuneDepozit.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GestiuneDepozit.Modules.Config
{
    /// <summary>
    /// Interaction logic for ConfigModule.xaml
    /// </summary>
    public partial class ConfigModule : UserControl
    {
        private readonly IServiceProvider ServiceProvider;
        private IServiceScope ServiceScope { get; set; }

        private readonly AppDbContext Db;

        public ConfigModule(IServiceProvider serviceProvider, AppDbContext db)
        {
            ServiceProvider = serviceProvider;
            ServiceScope = ServiceProvider.CreateScope();
            Db = db;

            InitializeComponent();

            MSSQL_IsTrustedConnection.IsChecked = Configuration.Parameters.IsTrustedConnection;
            MSSQL_Address.Text = Configuration.Parameters.ServerAddress.Decrypt();
            MSSQL_Database.Text = Configuration.Parameters.Database.Decrypt();
            MSSQL_Username.Text = Configuration.Parameters.Username.Decrypt();
            MSSQL_Password.Password = Configuration.Parameters.Password.Decrypt();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (MSSQL_Address.Text.Trim().Length > 3 && MSSQL_Database.Text.Trim().Length > 2)
            {
                Configuration.Parameters.FirstConfiguration = true;
                Configuration.Parameters.IsTrustedConnection = (bool)MSSQL_IsTrustedConnection.IsChecked;
                Configuration.Parameters.ServerAddress = MSSQL_Address.Text.Encrypt();
                Configuration.Parameters.Database = MSSQL_Database.Text.Encrypt();
                Configuration.Parameters.Username = MSSQL_Username.Text.Encrypt();
                Configuration.Parameters.Password = MSSQL_Password.Password.Encrypt();

                bool DbConnectionSuccesfully = false;

                try
                {
                    Db.Database.Migrate();
                    DbConnectionSuccesfully = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare creare/actualizare baza de date!" + Environment.NewLine + ex.Message, "Eroare conexiune baza de date!", MessageBoxButton.OK, MessageBoxImage.Stop);
                }

                if (DbConnectionSuccesfully)
                {
                    Configuration.WriteConfigFile();

                    ServiceScope?.Dispose();
                    ServiceScope = ServiceProvider.CreateScope();
                    var mainWindow = ServiceScope.ServiceProvider.GetService<MainWindow>();
                    mainWindow.LoadModuleSelection();
                }
            }
            else
            {
                MessageBox.Show("Completati toate campurile corespunzator!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (Configuration.Parameters.FirstConfiguration == true)
            {
                ServiceScope?.Dispose();
                ServiceScope = ServiceProvider.CreateScope();
                var mainWindow = ServiceScope.ServiceProvider.GetService<MainWindow>();
                mainWindow.LoadModuleSelection();
            }
            else
            {
                MessageBox.Show("Aplicatia se va inchide deoarece nu a fost configurata conexiunea cu baza de date cu succes!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                App.Current.Shutdown();
            }            
        }

        private void MSSQL_IsTrustedConnection_Checked(object sender, RoutedEventArgs e)
        {
            if (MSSQL_IsTrustedConnection.IsChecked.HasValue)
            {
                UserPanel.IsEnabled = !MSSQL_IsTrustedConnection.IsChecked.Value;
                PasswordPanel.IsEnabled = !MSSQL_IsTrustedConnection.IsChecked.Value;
            }            
        }

        private void MSSQL_IsTrustedConnection_Unchecked(object sender, RoutedEventArgs e)
        {
            if (MSSQL_IsTrustedConnection.IsChecked.HasValue)
            {
                UserPanel.IsEnabled = !MSSQL_IsTrustedConnection.IsChecked.Value;
                PasswordPanel.IsEnabled = !MSSQL_IsTrustedConnection.IsChecked.Value;
            }
        }
    }
}
