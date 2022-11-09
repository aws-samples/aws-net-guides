using Microsoft.EntityFrameworkCore;

namespace AspNetCoreWebApiRds.Data
{
    public class BillingContext : DbContext
    {
        public BillingContext(DbContextOptions<BillingContext> options) : base(options)
        {
        }

        public DbSet<Invoice>? Invoices { get; set; }
    }
}