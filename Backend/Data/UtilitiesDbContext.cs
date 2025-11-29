using Microsoft.EntityFrameworkCore;
using Backend.Models; // если у тебя есть папка Models с сущностями

namespace Backend.Data
{
    public class UtilitiesDbContext : DbContext
    {
        public UtilitiesDbContext(DbContextOptions<UtilitiesDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            base.OnModelCreating(modelBuilder);
        }
    }
}
