using System;
using System.Collections.Generic;
using System.Text;

namespace GestiuneDepozit.Data.Models
{
    public class Categorie
    {
        public int Id { get; set; }
        public string NumeCategorie { get; set; }

        public Status Status { get; set; }
    }
}
