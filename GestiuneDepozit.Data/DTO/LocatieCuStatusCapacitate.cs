using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GestiuneDepozit.Data.DTO
{
    public class LocatieCuStatusCapacitate
    {
        public int Id { get; set; }

        [Display(Name = "Nume locatie")]
        public string NumeLocatie { get; set; }

        [Display(Name = "Capacitate produse")]
        public int CapacitateProduse { get; set; }

        [Display(Name = "Produse pe locatie")]
        public int ProduseAlocate { get; set; }
    }
}
