using ClosedXML.Excel;
using GestiuneDepozit.Data;
using GestiuneDepozit.Data.DTO;
using GestiuneDepozit.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            DataContext = this;
            AfiseazaStatus();
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
                        .Include(i => i.Categorie)
                        .ThenInclude(c => c.Status)
                        .Select(x => new
                        {
                            Produs = x.CodProdus,
                            Seria = x.Serie,
                            Saptamana = x.Saptamana,
                            An = x.An,
                            Locatia = x.Locatie.NumeLocatie,
                            Categoria = x.Categorie.NumeCategorie,
                            Status = x.Categorie.Status.NumeStatus
                        })
                        .ToList();

                    worksheet.Cell(1, 1).Value = "Cod Produs";
                    worksheet.Cell(1, 2).Value = "Seria";
                    worksheet.Cell(1, 3).Value = "Saptamana";
                    worksheet.Cell(1, 4).Value = "An";
                    worksheet.Cell(1, 5).Value = "Locatia";
                    worksheet.Cell(1, 6).Value = "Categoria";
                    worksheet.Cell(1, 7).Value = "Status";

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
            var rez = Db.Produse.Include(i => i.Locatie).Include(i => i.Categorie).ThenInclude(c => c.Status).Where(w => w.CodProdus.Contains(SearchTxt.Text)).Select(x => new
            {
                Id = x.Id,
                x.InregistratDe,
                x.DataInregistrare,
                Produs = x.CodProdus,
                Seria = x.Serie,
                x.Saptamana,
                x.An,
                Locatia = x.Locatie.NumeLocatie,
                Status = x.Categorie.Status.NumeStatus
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
                .Select(
                    s => new LocatieCuStatusCapacitate
                    {
                        Id = s.Id,
                        NumeLocatie = s.NumeLocatie,
                        CapacitateProduse = s.CapacitateProduse,
                        ProduseAlocate = s.Produse.Count
                    }
                )
                .AsNoTracking()
                .ToList();

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
                case 4:
                    AfiseazaCategorie();
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

        private void AfiseazaStatus()
        {
            var status = Db.Status
                .AsNoTracking()
                .AsQueryable()
                .OrderBy(o => o.NumeStatus)
                .ToList();
            StatusGrid.ItemsSource = null;
            StatusGrid.ItemsSource = status;

            StatusCbx.Items.Clear();
            foreach (var item in status)
            {
                StatusCbx.Items.Add(item);
            }
            StatusCbx.DisplayMemberPath = "NumeStatus";
            StatusCbx.SelectedValuePath = "NumeStatus";
        }

        private void AfiseazaCategorie()
        {
            var categorii = Db.Categorii
                .AsNoTracking()
                .AsQueryable()
                .Include(i => i.Status)
                .OrderBy(o => o.NumeCategorie)
                .ToList()
                .Select(s => new CategorieCuNumeStatus
                {
                    Id = s.Id,
                    NumeCategorie = s.NumeCategorie,
                    NumeStatus = s.Status.NumeStatus
                });
            CategorieGrid.ItemsSource = null;
            CategorieGrid.ItemsSource = categorii;
        }

        private void AdaugaStatusBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(StatusTxt.Text))
            {
                MessageBox.Show("Verificati daca campurile sunt completate corespunzator!");
            }
            else
            {
                var status = Db.Status
                    .AsNoTracking()
                    .Where(w => w.NumeStatus == StatusTxt.Text)
                    .FirstOrDefault();

                if (status == null)
                {
                    var statusNou = new Status
                    {
                        NumeStatus = StatusTxt.Text
                    };
                    Db.Status.Add(statusNou);
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
            var statusGrid = ((Button)sender).Tag as Status;
            var status = Db.Status
                .Include(i=>i.Categorii)
                .Where(w => w.Id == statusGrid.Id)
                .FirstOrDefault();
            if (status != null)
            {
                var numarProduseStatus = Db.Produse.Where(w => w.Categorie.Status.Id == status.Id).Count();
                if (numarProduseStatus > 0)
                {
                    MessageBox.Show($"Statusul nu poate fi sters deoarece exista {numarProduseStatus} produse alocate cu acest status!", "", MessageBoxButton.OK, MessageBoxImage.Stop);

                    //ToDo: intreaba daca sa stearga si toate produsele
                }
                else
                {
                    if (MessageBox.Show($"Stergeti statusul cu numele {status.NumeStatus} si Id {status.Id}?", "Confirmare", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (status.Categorii.Count > 0)
                        {
                            Db.Categorii.RemoveRange(status.Categorii);
                        }
                        Db.Status.Remove(status);
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

        //ToDo: Fix adding category as the status is a tracked object
        private void AdaugaCategorieBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CategorieTxt.Text) || StatusCbx.SelectedItem == null || StatusCbx.SelectedItem is not Status)
            {
                MessageBox.Show("Verificati daca campurile sunt completate corespunzator!");
            }
            else
            {
                var selectedStatus = StatusCbx.SelectedItem as Status;
                var status = Db.Status
                    .AsQueryable()
                    .Include(i=>i.Categorii)
                    .Where(w => w.NumeStatus == selectedStatus.NumeStatus && w.Id == selectedStatus.Id)
                    .FirstOrDefault();
                if (status != null)
                {
                    var categorie = Db.Categorii.Include(i => i.Status).Where(w => w.NumeCategorie == CategorieTxt.Text).FirstOrDefault();
                    if (categorie == null)
                    {
                        var categorieNoua = new Categorie
                        {
                            NumeCategorie = CategorieTxt.Text,
                            Status = status
                        };

                        status.Categorii.Add(categorieNoua);
                        //Db.Entry(categorieNoua.Status).State = EntityState.Detached;

                        Db.Status.Update(status);
                        try
                        {
                            Db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Eroare la salvarea datelor in baza de date!" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        AfiseazaCategorie();
                        CategorieTxt.Clear();
                    }
                    else if (categorie.Status != status)
                    {
                        categorie.Status = status;

                        //Db.Entry(categorie.Status).State = EntityState.Detached;

                        Db.Categorii.Update(categorie);
                        try
                        {
                            Db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Eroare la salvarea datelor in baza de date!" + Environment.NewLine + ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        AfiseazaCategorie();
                        CategorieTxt.Clear();
                    }
                    else
                    {
                        MessageBox.Show($"Categoria este definita deja in sistem!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
        }

        private void CategorieGrid_Buton_Click(object sender, MouseButtonEventArgs e)
        {
            var categorieGrid = ((Button)sender).Tag as CategorieCuNumeStatus;
            var categorie = Db.Categorii.Where(w => w.Id == categorieGrid.Id).FirstOrDefault();
            if (categorie != null)
            {
                var numarProduseStatus = Db.Produse.Where(w => w.Categorie.Id == categorie.Id).Count();
                if (numarProduseStatus > 0)
                {
                    MessageBox.Show($"Categoria nu poate fi stearsa deoarece exista {numarProduseStatus} produse alocate cu acest status!", "", MessageBoxButton.OK, MessageBoxImage.Stop);

                    //ToDo: intreaba daca sa stearga si toate produsele
                }
                else
                {
                    if (MessageBox.Show($"Stergeti categoria cu numele {categorie.NumeCategorie} si Id {categorie.Id}?", "Confirmare", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Db.Categorii.Remove(categorie);
                        try
                        {
                            Db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Eroare la salvarea datelor in baza de date!" + Environment.NewLine + ex.Message, "Eroare", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        AfiseazaCategorie();
                    }
                }
            }
        }

        private void StatusCbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get the ComboBox.
            var comboBox = sender as ComboBox;

            // ... Set SelectedItem as Window Title.
            var value = comboBox?.SelectedItem as Status;
            //MessageBox.Show(value?.NumeStatus);
        }
    }
}
