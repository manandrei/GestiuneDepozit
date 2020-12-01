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
                Parameters parameters = new Parameters
                {
                    IsTrustedConnection = (bool)MSSQL_IsTrustedConnection.IsChecked,
                    ServerAddress = MSSQL_Address.Text.Encrypt(),
                    Database = MSSQL_Database.Text.Encrypt(),
                    Username = MSSQL_Username.Text.Encrypt(),
                    Password = MSSQL_Password.Password.Encrypt()
                };
                Configuration.Parameters = parameters;
                Configuration.WriteConfigFile();

                try
                {
                    Db.Database.Migrate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare creare/actualizare baza de date!" + Environment.NewLine + ex.Message, "Eroare conexiune baza de date!", MessageBoxButton.OK, MessageBoxImage.Stop);
                }

                ServiceScope?.Dispose();
                ServiceScope = ServiceProvider.CreateScope();
                var mainWindow = ServiceScope.ServiceProvider.GetService<MainWindow>();
                mainWindow.LoadModuleSelection();
            }
            else
            {
                MessageBox.Show("Completati toate campurile corespunzator!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ServiceScope?.Dispose();
            ServiceScope = ServiceProvider.CreateScope();
            var mainWindow = ServiceScope.ServiceProvider.GetService<MainWindow>();
            mainWindow.LoadModuleSelection();
        }

        private void MSSQL_IsTrustedConnection_Checked(object sender, RoutedEventArgs e)
        {
            MSSQL_PanelUserPass.IsEnabled = !(bool)MSSQL_IsTrustedConnection.IsChecked;
        }
    }
}
