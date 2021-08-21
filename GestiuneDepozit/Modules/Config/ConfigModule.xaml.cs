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
using GestiuneDepozit.Data.ServiceProviderContext;
using GestiuneDepozit.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GestiuneDepozit.Modules.Config
{
    /// <summary>
    /// Interaction logic for ConfigModule.xaml
    /// </summary>
    public partial class ConfigModule : UserControl
    {
        private IServiceProvider ServiceProvider;
        private IServiceScope ServiceScope { get; set; }

        private readonly string InitialDbType;

        public ConfigModule(IServiceProvider serviceProvider)
        {
            InitialDbType = Configuration.Parameters.DatabaseServerType;

            ServiceProvider = serviceProvider;
            ServiceScope = ServiceProvider.CreateScope();
            InitializeComponent();

            if (Configuration.Parameters.DatabaseServerType == ServerTypes.Sqlite)
            {
                DbSqlite.IsChecked = true;
                DbMSSQL.IsChecked = false;
                GroupMSSQL.Visibility = Visibility.Collapsed;
            }
            else
            {
                DbSqlite.IsChecked = false;
                GroupSqlite.Visibility = Visibility.Collapsed;
                DbMSSQL.IsChecked = true;
            }
            MSSQL_IsTrustedConnection.IsChecked = Configuration.Parameters.IsTrustedConnection;
            MSSQL_Address.Text = Configuration.Parameters.ServerAddress.Decrypt();
            MSSQL_Database.Text = Configuration.Parameters.Database.Decrypt();
            Sqlite_Database.Text = MSSQL_Database.Text;
            MSSQL_Username.Text = Configuration.Parameters.Username.Decrypt();
            MSSQL_Password.Password = Configuration.Parameters.Password.Decrypt();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {


            if (MSSQL_Address.Text.Trim().Length > 2 && MSSQL_Database.Text.Trim().Length > 2 && Sqlite_Database.Text.Trim().Length > 2)
            {
                Configuration.Parameters.FirstConfiguration = false;
                Configuration.Parameters.IsTrustedConnection = (bool)MSSQL_IsTrustedConnection.IsChecked;
                Configuration.Parameters.ServerAddress = MSSQL_Address.Text.Encrypt();
                Configuration.Parameters.Username = MSSQL_Username.Text.Encrypt();
                Configuration.Parameters.Password = MSSQL_Password.Password.Encrypt();


                if (DbMSSQL.IsChecked == true)
                {
                    Configuration.Parameters.DatabaseServerType = ServerTypes.SqlServer;
                    Configuration.Parameters.Database = MSSQL_Database.Text.Encrypt();
                }
                else
                {
                    Configuration.Parameters.DatabaseServerType = ServerTypes.Sqlite;
                    Configuration.Parameters.Database = Sqlite_Database.Text.Encrypt();
                }

                Configuration.WriteConfigFile();

                if (InitialDbType != Configuration.Parameters.DatabaseServerType)
                {
                    MessageBox.Show("Conexiunea aplicatiei s-a modificat iar aplicatia se va restarta acum pentru aplicarea noilor parametri!" + Environment.NewLine + "Repornirea aplicatiei poate dura putin timp (5-10 secunde)!", "Restart", MessageBoxButton.OK, MessageBoxImage.Warning);
                    App.RestartApplication();
                }
                else
                {
                    //Db migration
                    try
                    {
                        ServiceScope?.Dispose();
                        ServiceScope = ServiceProvider.CreateScope();
                        var Db = ServiceScope.ServiceProvider.GetService<AppDbContext>();
                        if (Db.Database.GetPendingMigrations().Count() > 0)
                        {
                            Db.Database.Migrate();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Eroare creare/actualizare baza de date!" + Environment.NewLine + ex.Message, "Eroare conexiune baza de date!", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
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
            if (Configuration.Parameters.FirstConfiguration == false)
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

        private void DbSqlite_Checked(object sender, RoutedEventArgs e)
        {
            if (DbSqlite.IsChecked.Value == true)
            {
                if (GroupMSSQL != null)
                {
                    GroupMSSQL.Visibility = Visibility.Collapsed;
                    GroupSqlite.Visibility = Visibility.Visible;
                }
            }
        }

        private void DbMSSQL_Checked(object sender, RoutedEventArgs e)
        {
            if (DbMSSQL.IsChecked.Value == true)
            {
                if (GroupSqlite != null)
                {
                    GroupMSSQL.Visibility = Visibility.Visible;
                    GroupSqlite.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
