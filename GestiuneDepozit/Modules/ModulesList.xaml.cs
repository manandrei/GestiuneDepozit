using GestiuneDepozit.Modules.Gestionar;
using GestiuneDepozit.Modules.Scan;
using Microsoft.Extensions.DependencyInjection;
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

namespace GestiuneDepozit.Modules
{
    /// <summary>
    /// Interaction logic for ModulesList.xaml
    /// </summary>
    public partial class ModulesList : UserControl
    {
        private readonly IServiceProvider ServiceProvider;
        private IServiceScope ServiceScope { get; set; }

        public ModulesList(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            ServiceScope = ServiceProvider.CreateScope();

            InitializeComponent();
        }

        private void ScanBtn_Click(object sender, RoutedEventArgs e)
        {
            ServiceScope?.Dispose();
            ServiceScope = ServiceProvider.CreateScope();
            var mainWindow = ServiceScope.ServiceProvider.GetService<MainWindow>();
            mainWindow.LoadScanModule();
        }

        private void GestionarBtn_Click(object sender, RoutedEventArgs e)
        {
            ServiceScope?.Dispose();
            ServiceScope = ServiceProvider.CreateScope();
            var mainWindow = ServiceScope.ServiceProvider.GetService<MainWindow>();
            mainWindow.LoadGestionarModule();
        }
    }
}
