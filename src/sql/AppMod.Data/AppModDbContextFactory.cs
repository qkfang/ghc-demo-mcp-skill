using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AppMod.Data;

public class AppModDbContextFactory : IDesignTimeDbContextFactory<AppModDbContext>
{
    public AppModDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppModDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=AppModDemo;Trusted_Connection=True;TrustServerCertificate=True;");

        return new AppModDbContext(optionsBuilder.Options);
    }
}
