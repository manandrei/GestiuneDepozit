using GestiuneDepozit.Data;
using GestiuneDepozit.Data.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestiuneDepozit.Services
{
    public class ScanHandler
    {
        public string ScannedValue { get; set; }
        public bool IsBarcodeValid { get => BarcodeValidation(); }

        public Produs ProdusScanat { get => InterpretBarcode(); }

        public Categorie Categorie { get; set; }

        private bool BarcodeValidation()
        {
            if (string.IsNullOrEmpty(ScannedValue))
            {
                return false;
            }
            else
            {
                //ToDo: Add property in configuration
                if (ScannedValue.Length == 20)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private Produs InterpretBarcode()
        {
            if (IsBarcodeValid)
            {
                var codcitit = ScannedValue.Substring(0, 10);
                var seriecitita = ScannedValue.Substring(10, 4);
                var saptamanacitita = ScannedValue.Substring(14, 2);
                var ancitit = ScannedValue.Substring(16, 4);
                if (codcitit.Length == 10 && seriecitita.Length == 4 && saptamanacitita.Length == 2 && ancitit.Length == 4)
                {
                    return new Produs
                    {
                        DataInregistrare = DateTime.Now,
                        InregistratDe = Environment.UserName,
                        CodProdus = codcitit,
                        Serie = seriecitita,
                        Saptamana = saptamanacitita,
                        An = ancitit
                    };
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
