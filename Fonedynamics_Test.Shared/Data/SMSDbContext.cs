using Fonedynamics_Test.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fonedynamics_Test.Shared.Data
{
    public class SMSDbContext : DbContext
    {
        public SMSDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<ProcessedSMS> ProcessedSMS { get; set; }
    }
}
