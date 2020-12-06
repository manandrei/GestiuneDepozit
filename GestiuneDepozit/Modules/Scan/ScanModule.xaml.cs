using GestiuneDepozit.Data;
using GestiuneDepozit.Data.Models;
using Microsoft.EntityFrameworkCore;
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

        private readonly AppDbContext Db;

        public ScanModule(IServiceProvider serviceProvider, AppDbContext db)
        {
            ServiceProvider = serviceProvider;
            ServiceScope = ServiceProvider.CreateScope();
            Db = db;
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            StatusLbl.Text = "Scanati codul de bare";
            QRImage.Visibility = Visibility.Visible;
            BarcodeInput.Focus();

            var categorii = Db.Categorii.AsQueryable().Include(i => i.Status).AsNoTracking().ToList();
            CategorieLst.ItemsSource = categorii;
        }

        private void BarcodeInput_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //BarcodeInput.Background = Brushes.GreenYellow;
            ScanerImageBackground.Background = Brushes.GreenYellow;
            StatusLbl.Text = "Scanati codul de bare";
            QRImage.Visibility = Visibility.Visible;
            BarcodeInput.Focus();
            BarcodeInput.SelectAll();
        }

        private void BarcodeInput_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //BarcodeInput.Background = Brushes.Red;
            ScanerImageBackground.Background = Brushes.Red;
            StatusLbl.Text = "Click pe campul de de mai jos pentru a putea efectua operatia de scanare!";
            QRImage.Visibility = Visibility.Hidden;
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
                        //ToDo: Check if selectedItem is not null
                        if (CategorieLst.SelectedItem != null && CategorieLst.SelectedItem is Categorie)
                        {
                            var categorieSelectata = CategorieLst.SelectedItem as Categorie;
                            ScanHandler.Categorie = categorieSelectata;


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
                            var recordexist = Db.Produse.AsQueryable().Where(w => w.CodProdus == ScanHandler.ProdusScanat.CodProdus && w.Serie == ScanHandler.ProdusScanat.Serie).AsNoTracking().FirstOrDefault();
                            if (recordexist == null)
                            {
                                MessageBox.Show("Selectati o categorie si apoi scanati din nou");
                                BarcodeInput.Clear();
                            }
                            else
                            {
                                var gestiuneProdus = ServiceScope.ServiceProvider.GetService<GestiuneProdus>();
                                if (gestiuneProdus != null)
                                {
                                    gestiuneProdus.ScanHandler = ScanHandler;
                                    var mainWindow = ServiceScope.ServiceProvider.GetService<MainWindow>();
                                    mainWindow.MainPanel.Children.Add(gestiuneProdus);
                                    mainWindow.MainPanel.Children.Remove(this);
                                }
                            }
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

        private void CategorieLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BarcodeInput.Focus();
            BarcodeInput.SelectAll();
        }
    }
}
