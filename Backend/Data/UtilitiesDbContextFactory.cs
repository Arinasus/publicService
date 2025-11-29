using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Backend.Models;

namespace Backend.Data
{
    public class UtilitiesDbContextFactory : IDesignTimeDbContextFactory<UtilitiesDbContext>
    {
        public UtilitiesDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UtilitiesDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=utilities;Username=util_user;Password=util_pass");

            return new UtilitiesDbContext(optionsBuilder.Options);
        }
    }
}
