using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GestiuneDepozit.Data.Models
{
    public class Produs
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime DataInregistrare { get; set; } = DateTime.Now;
        public string InregistratDe { get; set; }
        public string CodProdus { get; set; }
        public string Serie { get; set; }
        public string Saptamana { get; set; }
        public string An { get; set; }
        public Locatie Locatie { get; set; }
        public Categorie Categorie { get; set; }
    }
}
