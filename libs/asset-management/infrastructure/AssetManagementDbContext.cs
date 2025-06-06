using MicraPro.AssetManagement.Domain.StorageAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MicraPro.AssetManagement.Infrastructure;

public class AssetManagementDbContext(IConfiguration? configuration = null) : DbContext
{
    public DbSet<AssetDb> AssetEntries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(configuration?.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assetEntity = modelBuilder.Entity<AssetDb>();
        assetEntity.HasKey(e => e.Id);
        assetEntity.Property(e => e.RelativePath).IsRequired();
    }
}
