using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GestiuneDepozit.Data.Models
{
    [Index(nameof(NumeStatus), IsUnique = true)]
    public class Status
    {
        public int Id { get; set; }
        public string NumeStatus { get; set; }
        public List<Categorie> Categorii { get; set; }
    }
}
