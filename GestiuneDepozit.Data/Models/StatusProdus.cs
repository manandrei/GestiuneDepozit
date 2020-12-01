using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestiuneDepozit.Data.Models
{
    [Index(nameof(Status), IsUnique = true)]
    public class StatusProdus
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }
}
