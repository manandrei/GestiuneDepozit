using GestiuneDepozit.Data;
using Microsoft.EntityFrameworkCore;
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

namespace GestiuneDepozit.Modules.Scan
{
    /// <summary>
    /// Interaction logic for GasesteProdus.xaml
    /// </summary>
    public partial class GasesteProdus : UserControl
    {
        public string ProductCodePartial { get; set; }
        private readonly AppDbContext Db;

        private readonly IServiceProvider ServiceProvider;
        public IServiceScope ServiceScope { get; set; }

        public GasesteProdus(AppDbContext db, IServiceProvider serviceProvider)
        {
            Db = db;
            ServiceProvider = serviceProvider;
            ServiceScope = ServiceProvider.CreateScope();
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SearchValueLbl.Text = ProductCodePartial;

            if (!string.IsNullOrEmpty(ProductCodePartial))
            {
                var produse = Db.Produse.Include(i => i.Locatie).Include(i => i.Status).Where(w => w.CodProdus.EndsWith(ProductCodePartial)).OrderBy(o => o.DataInregistrare).Select(s => new
                {
                    s.Id,
                    s.DataInregistrare,
                    s.InregistratDe,
                    s.CodProdus,
                    s.Serie,
                    s.Saptamana,
                    s.An,
                    s.Locatie.NumeLocatie,
                    s.Status.Status
                }).ToList();

                RezultatGrid.ItemsSource = produse;
            }

            BtnOK.Focus();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            ServiceScope.Dispose();
            ServiceScope = ServiceProvider.CreateScope();
            var mainWindow = ServiceScope.ServiceProvider.GetService<MainWindow>();
            mainWindow.LoadScanModule();
        }
    }
}
