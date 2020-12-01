using GestiuneDepozit.Modules;
using GestiuneDepozit.Modules.Config;
using GestiuneDepozit.Modules.Gestionar;
using GestiuneDepozit.Modules.Scan;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace GestiuneDepozit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly IServiceProvider ServiceProvider;
        public IServiceScope ServiceScope { get; set; }

        private readonly ModulesList _modulesList;


        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Visibility displayMenuModuleSelection = Visibility.Hidden;


        public Visibility DisplayMenuModuleSelection
        {
            get => displayMenuModuleSelection; set
            {
                displayMenuModuleSelection = value;
                NotifyPropertyChanged();
            }
        }

        public MainWindow(IServiceProvider serviceProvider, ModulesList modulesList)
        {
            ServiceProvider = serviceProvider;
            ServiceScope = ServiceProvider.CreateScope();

            _modulesList = modulesList;

            //DataContext = this;

            InitializeComponent();

            if (!Configuration.ConfigFileLoaded)
            {
                LoadConfigModule();
            }
            else
            {
                LoadModuleSelection();
            }
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void MenuListModules_Click(object sender, RoutedEventArgs e)
        {
            LoadModuleSelection();
        }

        public void LoadConfigModule()
        {
            ClearMainContainer();
            var configModule = ServiceScope.ServiceProvider.GetService<ConfigModule>();
            MainPanel.Children.Add(configModule);
            DisplayMenuModuleSelection = Visibility.Visible;
        }

        public void LoadModuleSelection()
        {
            ClearMainContainer();
            var PanouModule = ServiceScope.ServiceProvider.GetService<ModulesList>();
            MainPanel.Children.Add(PanouModule);
            DisplayMenuModuleSelection = Visibility.Hidden;
            //DisplayMenuListModules();
        }

        public void LoadScanModule()
        {
            ClearMainContainer();
            var ModulScanare = ServiceScope.ServiceProvider.GetService<ScanModule>();
            MainPanel.Children.Add(ModulScanare);
            DisplayMenuModuleSelection = Visibility.Visible;
        }

        public void LoadGestionarModule()
        {
            ClearMainContainer();
            var ModulGestionar = ServiceScope.ServiceProvider.GetService<GestionarModule>();
            MainPanel.Children.Add(ModulGestionar);
            DisplayMenuModuleSelection = Visibility.Visible;
        }

        public void ClearMainContainer()
        {

            ServiceScope.Dispose();

            MainPanel.Children.Clear();

            GC.Collect();

            ServiceScope = ServiceProvider.CreateScope();
        }

        private void MenuConfig_Click(object sender, RoutedEventArgs e)
        {
            ClearMainContainer();
            var configModule = ServiceScope.ServiceProvider.GetService<ConfigModule>();
            MainPanel.Children.Add(configModule);
            DisplayMenuModuleSelection = Visibility.Visible;
        }
    }
}
