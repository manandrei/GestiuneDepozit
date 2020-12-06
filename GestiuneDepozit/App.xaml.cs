using GestiuneDepozit.Data;
using GestiuneDepozit.Modules;
using GestiuneDepozit.Modules.Config;
using GestiuneDepozit.Modules.Gestionar;
using GestiuneDepozit.Modules.Scan;
using GestiuneDepozit.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
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

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>();
            services.AddSingleton<MainWindow>();
            services.AddTransient<AboutWindow>();
            services.AddTransient<ConfigModule>();
            services.AddTransient<ModulesList>();
            services.AddTransient<GestionarModule>();
            services.AddTransient<ScanModule>();
            services.AddTransient<ScanHandler>();
            services.AddTransient<GestiuneProdus>();
            services.AddTransient<GasesteProdus>();
        }

        private void AppOnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}
