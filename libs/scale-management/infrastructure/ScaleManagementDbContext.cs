using MicraPro.ScaleManagement.Domain.StorageAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MicraPro.ScaleManagement.Infrastructure;

internal class ScaleManagementDbContext(IConfiguration? configuration = null) : DbContext
{
    public DbSet<ScaleDb> ScaleEntries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(configuration?.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ScaleDb>();
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Identifier).IsRequired();
        entity.Property(e => e.Name).IsRequired();
        entity.Property(e => e.ImplementationType).IsRequired();
    }
}
