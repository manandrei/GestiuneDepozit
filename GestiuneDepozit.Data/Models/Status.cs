using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestiuneDepozit.Data.Models
{
    [Index(nameof(NumeStatus), IsUnique = true)]
    public class Status
    {
        public int Id { get; set; }
        public string NumeStatus { get; set; }
    }
}
