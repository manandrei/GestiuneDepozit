using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestiuneDepozit.Data.Models
{
    [Index(nameof(NumeLocatie), IsUnique = true)]
    public class Locatie
    {
        public int Id { get; set; }
        public string NumeLocatie { get; set; }
        public int CapacitateProduse { get; set; }
        public List<Produs> Produse { get; set; }
    }
}