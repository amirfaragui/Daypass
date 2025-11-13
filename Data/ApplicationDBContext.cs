using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace DayPass.Data
{
    public class ApplicationDBContext : DbContext
    {
        public DbSet<CONGBARCODE> CONGBARCODE { get; set; }
        public DbSet<UDBMOVEMENT> UDBMOVEMENT { get; set; }
        public DbSet<DEVINFO> DEVINFO { get; set; }

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
           : base(options)
        {
        }
    
     public override void Dispose()
        {
            base.Dispose();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
          //  builder.Entity<CONGBARCODE>().OwnsOne(x => x.CepanDetails);
            base.OnModelCreating(builder);
        }

    }
}
