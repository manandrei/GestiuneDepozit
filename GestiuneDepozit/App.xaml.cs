using GestiuneDepozit.Data;
using GestiuneDepozit.Data.ServiceProviderContext;
using GestiuneDepozit.Modules;
using GestiuneDepozit.Modules.Config;
using GestiuneDepozit.Modules.Gestionar;
using GestiuneDepozit.Modules.Scan;
using GestiuneDepozit.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace GestiuneDepozit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;
        public App()
        {
            CultureInfo RoCultureCustom = new CultureInfo("ro");
            RoCultureCustom.NumberFormat.NumberDecimalSeparator = ".";
            RoCultureCustom.NumberFormat.NumberGroupSeparator = " ";

            Thread.CurrentThread.CurrentCulture = RoCultureCustom;
            Thread.CurrentThread.CurrentUICulture = RoCultureCustom;
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                        XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            var services = new ServiceCollection();
            ConfigureServices(services);

            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            if (Configuration.Parameters.DatabaseServerType == ServerTypes.SqlServer)
            {
                services.AddDbContext<AppDbContext, SqlServerContext>();
            }
            else
            {
                services.AddDbContext<AppDbContext, SqliteContext>();
            }

            services.AddSingleton<MainWindow>();
            services.AddTransient<AboutWindow>();
            services.AddTransient<ConfigModule>();
            services.AddTransient<ModulesList>();
            services.AddTransient<GestionarModule>();
            services.AddTransient<ScanModule>();
            services.AddTransient<ScanHandler>();
            services.AddTransient<GestiuneProdus>();
            services.AddTransient<GasesteProdus>();
            services.AddTransient<LicenseWindow>();
        }

        private void AppOnStartup(object sender, StartupEventArgs e)
        {
            if (Configuration.Parameters.AcceptedEULA)
            {
                //Db migration
                try
                {
                    var Db = _serviceProvider.GetService<AppDbContext>();
                    if (Db.Database.GetPendingMigrations().Count() > 0)
                    {
                        Db.Database.Migrate();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare creare/actualizare baza de date!" + Environment.NewLine + ex.Message, "Eroare conexiune baza de date!", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                var mainWindow = _serviceProvider.GetService<MainWindow>();
                mainWindow?.Show();
            }
            else
            {
                var licenseWindow = _serviceProvider.GetService<LicenseWindow>();
                licenseWindow?.Show();
            }
        }

        public static void RestartApplication()
        {
            ProcessStartInfo Info = new ProcessStartInfo
            {
                Arguments = "/C choice /C Y /N /D Y /T 4 & START \"\" \"" + Process.GetCurrentProcess().MainModule.FileName + "\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            };

            Process.Start(Info);
            //Process.GetCurrentProcess().Kill();
            App.Current.Shutdown();
        }
    }
}
