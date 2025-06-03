using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MicraPro.Shared.Infrastructure;

internal class SharedDbContext(IConfiguration? configuration = null) : DbContext
{
    public DbSet<ConfigurationEntry> ConfigurationEntries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(configuration?.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConfigurationEntry>().HasKey(e => e.Key);
    }
}
