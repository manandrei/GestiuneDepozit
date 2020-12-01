using GestiuneDepozit.Data;
using GestiuneDepozit.Services;
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
    /// Interaction logic for ScanAction.xaml
    /// </summary>
    public partial class GestiuneProdus : UserControl
    {
        private readonly IServiceProvider ServiceProvider;
        public IServiceScope ServiceScope { get; set; }

        public ScanHandler ScanHandler { get; set; }
        public readonly AppDbContext Db;

        public GestiuneProdus(AppDbContext db, IServiceProvider serviceProvider)
        {
            Db = db;
            ServiceProvider = serviceProvider;
            ServiceScope = ServiceProvider.CreateScope();
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ScanHandler != null)
            {
                if (ScanHandler.IsBarcodeValid)
                {
                    IdLbl.Content = ScanHandler.ProdusScanat.Id.ToString();
                    DataLbl.Content = ScanHandler.ProdusScanat.DataInregistrare.ToString("dd.MM.yyyy HH:mm:ss");
                    UsernameLbl.Content = ScanHandler.ProdusScanat.InregistratDe;
                    CodProdusLbl.Content = ScanHandler.ProdusScanat.CodProdus;
                    SeriaLbl.Content = ScanHandler.ProdusScanat.Serie;
                    SaptamanaLbl.Content = ScanHandler.ProdusScanat.Saptamana;
                    AnLbl.Content = ScanHandler.ProdusScanat.An;

                    var DbRecord = Db.Produse
                        .Include(i => i.Locatie)
                        .Include(i => i.Status)
                        .Where(w =>
                            w.CodProdus == ScanHandler.ProdusScanat.CodProdus &&
                            w.Serie == ScanHandler.ProdusScanat.Serie &&
                            w.Saptamana == ScanHandler.ProdusScanat.Saptamana &&
                            w.An == ScanHandler.ProdusScanat.An
                            )
                        .FirstOrDefault();
                    if (DbRecord != null)
                    {
                        InfoLbl.Content = "S-a sters din stoc produsul cu detaliile de mai jos";
                        InfoLbl.Background = Brushes.Red;

                        Db.Produse.Remove(DbRecord);
                        try
                        {
                            Db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Eroare la salvarea datelor in baza de date!" + Environment.NewLine + ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        
                        IdLbl.Content = DbRecord.Id.ToString();
                        LocatiaLbl.Content = DbRecord.Locatie.NumeLocatie;
                        StatusLbl.Content = DbRecord.Status.Status;
                    }
                    else
                    {
                        InfoLbl.Content = "S-a adaugat in stoc produsul cu detaliile de mai jos";
                        InfoLbl.Background = Brushes.GreenYellow;

                        var record = ScanHandler.ProdusScanat;

                        //var locatieProdusExistent = Db.Locatii
                        //    .Include(
                        //        i => i.Produse
                        //            .Where(w => w.CodProdus == record.CodProdus)
                        //        )
                        //    .Where(w => w.Produse.Count < w.CapacitateProduse && w.Produse.Count > 0)
                        //    .OrderByDescending(o => o.CapacitateProduse - o.Produse.Count)
                        //    .FirstOrDefault();

                        var locatieProdusExistent = Db.Produse
                            .Include(i => i.Locatie)
                            .ThenInclude(i => i.Produse)
                            .Where(w => w.CodProdus == record.CodProdus)
                            .Select(s => s.Locatie)
                            .Where(w => w.Produse.Count > 0 && w.Produse.Count < w.CapacitateProduse)
                            .FirstOrDefault();

                        record.Status = ScanHandler.Status;

                        if (locatieProdusExistent != null)
                        {
                            record.Locatie = locatieProdusExistent;
                        }
                        else
                        {
                            var locatieGoala = Db.Locatii
                            .Include(i => i.Produse)
                            .Where(w => w.Produse.Count == 0)
                            .OrderBy(o => o.NumeLocatie)
                            .FirstOrDefault();

                            if (locatieGoala != null)
                            {
                                record.Locatie = locatieGoala;
                            }
                            else
                            {
                                var locatieDisponibila = Db.Locatii
                                    .Include(i => i.Produse)
                                    .Where(w => w.Produse.Count < w.CapacitateProduse)
                                    .OrderBy(o => o.NumeLocatie)
                                    .FirstOrDefault();

                                if (locatieDisponibila != null)
                                {
                                    record.Locatie = locatieDisponibila;
                                }
                                else
                                {
                                    //ToDo: Afiseaza mesaj ca nu sunt locatii de depozitare sau posibilitatea de a alege manual
                                    IdLbl.Content = "";
                                    InfoLbl.Content = "Nu mai sunt locatii pentru depozitare!";
                                    InfoLbl.Background = Brushes.Red;
                                    GestiuneContent.Background = Brushes.Red;

                                    MessageBox.Show("Nu mai sunt locatii pentru depozitare!", "Alerta!", MessageBoxButton.OK, MessageBoxImage.Error);
                                    BtnOK.Focus();
                                    return;
                                }
                            }
                        }

                        Db.Entry(record.Status).State = EntityState.Unchanged;

                        Db.Produse.Add(record);
                        try
                        {
                            Db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Eroare la salvarea datelor in baza de date!" + Environment.NewLine + ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }

                        IdLbl.Content = record.Id.ToString();
                        LocatiaLbl.Content = record.Locatie.NumeLocatie;
                        StatusLbl.Content = record.Status.Status;

                    }
                }
                else
                {
                    MessageBox.Show("Codul de bare nu va putea fi procesat!");
                }
            }
            BtnOK.Focus();

            //ToDo: create a list with the last 20 actions
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
