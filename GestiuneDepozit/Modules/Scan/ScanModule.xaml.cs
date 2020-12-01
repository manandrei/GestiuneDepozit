using GestiuneDepozit.Data.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace GestiuneDepozit.Modules.Scan
{
    /// <summary>
    /// Interaction logic for ScanModule.xaml
    /// </summary>
    public partial class ScanModule : UserControl
    {
        private readonly IServiceProvider ServiceProvider;
        public IServiceScope ServiceScope { get; set; }

        public ScanModule(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            ServiceScope = ServiceProvider.CreateScope();

            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            StatusLbl.Text = "Scanati codul de bare";
            BarcodeInput.Focus();
        }

        private void BarcodeInput_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //BarcodeInput.Background = Brushes.GreenYellow;
            ScanerImageBackground.Background = Brushes.GreenYellow;
            StatusLbl.Text = "Scanati codul de bare";
            BarcodeInput.Focus();
            BarcodeInput.SelectAll();
        }

        private void BarcodeInput_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //BarcodeInput.Background = Brushes.Red;
            ScanerImageBackground.Background = Brushes.Red;
            StatusLbl.Text = "Click pe campul de de mai jos pentru a putea efectua operatia de scanare!";
        }

        private void BarcodeInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                e.Handled = true;
                //run barcode data read function
                ServiceScope.Dispose();
                ServiceScope = ServiceProvider.CreateScope();
                var ScanHandler = ServiceScope.ServiceProvider.GetRequiredService<Services.ScanHandler>();
                if (ScanHandler != null)
                {
                    ScanHandler.ScannedValue = BarcodeInput.Text.Trim();
                    if (ScanHandler.IsBarcodeValid)
                    {
                        //Todo: Afiseaza fereastra pentru a selecta un status al produsului si modifica mai jos
                        StatusProdus status = null; //Db.StatusProdus.Where(w => w.Status == "testare").FirstOrDefault();
                        if (status == null)
                        {
                            status = new StatusProdus { Id = 1, Status = "bune" };
                        }
                        ScanHandler.Status = status;


                        var gestiuneProdus = ServiceScope.ServiceProvider.GetService<GestiuneProdus>();
                        if (gestiuneProdus != null)
                        {
                            gestiuneProdus.ScanHandler = ScanHandler;
                            var mainWindow = ServiceScope.ServiceProvider.GetService<MainWindow>();
                            mainWindow.MainPanel.Children.Add(gestiuneProdus);
                            mainWindow.MainPanel.Children.Remove(this);
                        }
                    }
                    else
                    {
                        if (BarcodeInput.Text.Trim().Length > 2)
                        {
                            //ToDo: Incearca sa caute produsul pentru afisarea locatiei
                            var gasesteProdus = ServiceScope.ServiceProvider.GetService<GasesteProdus>();
                            gasesteProdus.ProductCodePartial = BarcodeInput.Text.Trim();
                            var mainWindow = ServiceScope.ServiceProvider.GetService<MainWindow>();
                            mainWindow.MainPanel.Children.Add(gasesteProdus);
                            mainWindow.MainPanel.Children.Remove(this);

                            //var frmGaseste = ServiceScope.ServiceProvider.GetService<FrmGasesteProdus>();
                            //if (frmGaseste != null)
                            //{
                            //    frmGaseste.ProductCodePartial = BarcodeInput.Text.Trim();
                            //    frmGaseste.ShowDialog(this.ParentForm);
                            //}
                        }
                    }
                }
                BarcodeInput.Clear();
            }
        }
    }
}
