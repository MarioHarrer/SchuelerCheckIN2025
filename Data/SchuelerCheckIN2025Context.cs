using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchuelerCheckIN2025.Models;

namespace SchuelerCheckIN2025.Data
{
    public class SchuelerCheckIN2025Context : DbContext
    {
        public SchuelerCheckIN2025Context (DbContextOptions<SchuelerCheckIN2025Context> options)
            : base(options)
        {
        }

        public DbSet<SchuelerCheckIN2025.Models.Schuelerdaten> Schuelerdaten { get; set; } = default!;
    }
}
