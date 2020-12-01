using ClosedXML.Excel;
using GestiuneDepozit.Data;
using GestiuneDepozit.Data.DTO;
using GestiuneDepozit.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace GestiuneDepozit.Modules.Gestionar
{
    /// <summary>
    /// Interaction logic for GestionarModule.xaml
    /// </summary>
    public partial class GestionarModule : UserControl
    {
        private readonly AppDbContext Db;
        private readonly IServiceProvider ServiceProvider;
        private IServiceScope ServiceScope { get; set; }

        public GestionarModule(AppDbContext db, IServiceProvider serviceProvider)
        {
            Db = db;
            ServiceProvider = serviceProvider;
            ServiceScope = ServiceProvider.CreateScope();

            InitializeComponent();

        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel file (*.xlsx)|*.xlsx";
            saveFileDialog.FileName = $"Stoc {DateTime.Now.Date.ToString("dd.MM.yyyy")}.xlsx";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (saveFileDialog.ShowDialog() == true)
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"Stoc {DateTime.Now.Date.ToString("dd.MM.yyyy")}");
                    var stoc = Db.Produse
                        .Include(i => i.Locatie)
                        .Include(i => i.Status)
                        .Select(x => new
                        {
                            Produs = x.CodProdus,
                            Seria = x.Serie,
                            Saptamana = x.Saptamana,
                            An = x.An,
                            Locatia = x.Locatie.NumeLocatie,
                            Status = x.Status.Status
                        })
                        .ToList();

                    worksheet.Cell(1, 1).Value = "Cod Produs";
                    worksheet.Cell(1, 2).Value = "Seria";
                    worksheet.Cell(1, 3).Value = "Saptamana";
                    worksheet.Cell(1, 4).Value = "An";
                    worksheet.Cell(1, 5).Value = "Locatia";
                    worksheet.Cell(1, 6).Value = "Status";

                    worksheet.Cell(2, 1).Value = stoc.AsEnumerable();

                    workbook.SaveAs(saveFileDialog.FileName);
                }
                if (File.Exists(saveFileDialog.FileName))
                {
                    Process p = new Process();
                    p.StartInfo.FileName = saveFileDialog.FileName;
                    p.StartInfo.UseShellExecute = true;
                    p.Start();
                }
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            var rez = Db.Produse.Include(i => i.Locatie).Include(i => i.Status).Where(w => w.CodProdus.Contains(SearchTxt.Text)).Select(x => new
            {
                Id = x.Id,
                x.InregistratDe,
                x.DataInregistrare,
                Produs = x.CodProdus,
                Seria = x.Serie,
                x.Saptamana,
                x.An,
                Locatia = x.Locatie.NumeLocatie,
                Status = x.Status.Status
            }).ToList();
            ResultGrid.ItemsSource = null;
            ResultGrid.ItemsSource = rez;
        }

        private void AdaugaLocatieBtn_Click(object sender, RoutedEventArgs e)
        {
            int capacitate;
            if (string.IsNullOrEmpty(NumeLocatieTxt.Text) || string.IsNullOrEmpty(CapacitateNum.Text) || int.TryParse(CapacitateNum.Text, out capacitate) == false)
            {
                MessageBox.Show("Verificati daca campurile sunt completate corespunzator!");
            }
            else
            {
                var loc = Db.Locatii.Where(w => w.NumeLocatie == NumeLocatieTxt.Text).FirstOrDefault();
                if (loc == null)
                {
                    var locNoua = new Locatie
                    {
                        NumeLocatie = NumeLocatieTxt.Text,
                        CapacitateProduse = capacitate
                    };
                    Db.Locatii.Add(locNoua);
                    try
                    {
                        Db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Eroare la salvarea datelor in baza de date!" + Environment.NewLine + ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                    AfiseazaLocatii();
                    NumeLocatieTxt.Clear();
                    CapacitateNum.Text = "0";
                }
                else
                {
                    if (loc.CapacitateProduse == capacitate)
                    {
                        MessageBox.Show($"Locatia este definita deja in sistem!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    if (MessageBox.Show($"Locatia este definita deja in sistem!{Environment.NewLine}Doriti sa actualizati valoarea capacitatii de la {loc.CapacitateProduse} la {capacitate}?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        loc.CapacitateProduse = capacitate;
                        Db.Locatii.Update(loc);
                        try
                        {
                            Db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Eroare la salvarea datelor in baza de date!" + Environment.NewLine + ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        AfiseazaLocatii();
                        NumeLocatieTxt.Clear();
                        CapacitateNum.Clear();
                    }
                }
            }
        }

        private void AfiseazaLocatii()
        {
            var locatii = Db.Locatii
                .Include(i => i.Produse)
                .OrderBy(o => o.NumeLocatie)
                .Select(s => new LocatieCuStatusCapacitate
                {
                    Id = s.Id,
                    NumeLocatie = s.NumeLocatie,
                    CapacitateProduse = s.CapacitateProduse,
                    ProduseAlocate = s.Produse.Count
                }
                ).ToList();

            LocatiiGrid.ItemsSource = null;
            LocatiiGrid.ItemsSource = locatii;
        }

        private void TabControlGestiune_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (TabControlGestiune.SelectedIndex)
            {
                case 2:
                    AfiseazaLocatii();
                    break;
                case 3:
                    AfiseazaStatus();
                    break;
                default:
                    break;
            }
        }

        private void LocatiiGrid_Buton_Click(object sender, MouseButtonEventArgs e)
        {
            var locGrid = ((Button)sender).Tag as LocatieCuStatusCapacitate;
            //var locGrid = (LocatieCuStatusCapacitate)LocatiiGrid.SelectedItem;
            var loc = Db.Locatii.Include(i => i.Produse).Where(w => w.Id == locGrid.Id).FirstOrDefault();
            if (loc != null)
            {
                if (loc.Produse.Count > 0)
                {
                    MessageBox.Show($"Locatia nu poate fi stearsa deoarece contine {loc.Produse.Count} produse alocate!", "", MessageBoxButton.OK, MessageBoxImage.Stop);

                    //ToDo: intreaba daca sa stearga si toate produsele
                }
                else
                {
                    if (MessageBox.Show($"Stergeti locatia cu numele {loc.NumeLocatie} si Id {loc.Id}?", "Confirmare", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Db.Locatii.Remove(loc);
                        try
                        {
                            Db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Eroare la salvarea datelor in baza de date!" + Environment.NewLine + ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        AfiseazaLocatii();
                    }
                }
            }
        }

        private async void AfiseazaStatus()
        {
            var status = await Db.StatusProdus
                .OrderBy(o => o.Status)
                .ToListAsync();
            StatusGrid.ItemsSource = null;
            StatusGrid.ItemsSource = status;
        }

        private async void AdaugaStatusBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(StatusTxt.Text))
            {
                MessageBox.Show("Verificati daca campurile sunt completate corespunzator!");
            }
            else
            {
                var status = await Db.StatusProdus.Where(w => w.Status == StatusTxt.Text).FirstOrDefaultAsync();
                if (status == null)
                {
                    var statusNou = new StatusProdus
                    {
                        Status = StatusTxt.Text
                    };
                    Db.StatusProdus.Add(statusNou);
                    await Db.SaveChangesAsync();
                    AfiseazaStatus();
                    StatusTxt.Clear();
                }
                else
                {
                    MessageBox.Show($"Statusul este definit deja in sistem!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private void StatusGrid_Buton_Click(object sender, MouseButtonEventArgs e)
        {
            var statusGrid = ((Button)sender).Tag as StatusProdus;
            var status = Db.StatusProdus.Where(w => w.Id == statusGrid.Id).FirstOrDefault();
            if (status != null)
            {
                var numarProduseStatus = Db.Produse.Where(w => w.Status.Id == status.Id).Count();
                if (numarProduseStatus > 0)
                {
                    MessageBox.Show($"Statusul nu poate fi sters deoarece exista {numarProduseStatus} produse alocate cu acest status!", "", MessageBoxButton.OK, MessageBoxImage.Stop);

                    //ToDo: intreaba daca sa stearga si toate produsele
                }
                else
                {
                    if (MessageBox.Show($"Stergeti statusul cu numele {status.Status} si Id {status.Id}?", "Confirmare", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Db.StatusProdus.Remove(status);
                        try
                        {
                            Db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Eroare la salvarea datelor in baza de date!" + Environment.NewLine + ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        AfiseazaStatus();
                    }
                }
            }
        }
    }
}
