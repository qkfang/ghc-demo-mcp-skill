using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GhcDemo.Sql;

public class GhcDemoSqlContextFactory : IDesignTimeDbContextFactory<GhcDemoSqlContext>
{
    public GhcDemoSqlContext CreateDbContext(string[] args)
    {
        var databasePath = Path.Combine(Directory.GetCurrentDirectory(), "ghc-demo-dev.db");
        var optionsBuilder = new DbContextOptionsBuilder<GhcDemoSqlContext>();

        optionsBuilder.UseSqlite($"Data Source={databasePath}");

        return new GhcDemoSqlContext(optionsBuilder.Options);
    }
}
