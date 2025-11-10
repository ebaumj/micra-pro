using MicraPro.BrewByWeight.Domain.StorageAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MicraPro.BrewByWeight.Infrastructure;

public class BrewByWeightDbContext(IConfiguration? configuration = null) : DbContext
{
    public DbSet<ProcessDb> ProcessEntries { get; set; }
    public DbSet<ProcessRuntimeDataDb> ProcessRuntimeDataEntries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(configuration?.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var processEntity = modelBuilder.Entity<ProcessDb>();
        processEntity.HasKey(e => e.Id);
        processEntity.Property(e => e.Timestamp).IsRequired();
        processEntity.Property(e => e.BeanId).IsRequired();
        processEntity.Property(e => e.InCupQuantity).IsRequired();
        processEntity.Property(e => e.GrindSetting).IsRequired();
        processEntity.Property(e => e.CoffeeQuantity).IsRequired();
        processEntity.Property(e => e.TargetExtractionTime).IsRequired();
        processEntity.Property(e => e.Spout).IsRequired();
        processEntity
            .HasMany(e => e.RuntimeData)
            .WithOne(e => e.Process)
            .HasForeignKey(e => e.ProcessId)
            .IsRequired();
        var finishedProcessEntity = modelBuilder.Entity<FinishedProcessDb>();
        finishedProcessEntity.Property(e => e.AverageFlow).IsRequired();
        finishedProcessEntity.Property(e => e.TotalQuantity).IsRequired();
        finishedProcessEntity.Property(e => e.ExtractionTime).IsRequired();
        finishedProcessEntity.HasBaseType<ProcessDb>();
        var cancelledProcessEntity = modelBuilder.Entity<CancelledProcessDb>();
        cancelledProcessEntity.Property(e => e.AverageFlow).IsRequired();
        cancelledProcessEntity.Property(e => e.TotalQuantity).IsRequired();
        cancelledProcessEntity.Property(e => e.TotalTime).IsRequired();
        cancelledProcessEntity.HasBaseType<ProcessDb>();
        var failedProcessEntity = modelBuilder.Entity<FailedProcessDb>();
        failedProcessEntity.Property(e => e.AverageFlow).IsRequired();
        failedProcessEntity.Property(e => e.TotalQuantity).IsRequired();
        failedProcessEntity.Property(e => e.TotalTime).IsRequired();
        failedProcessEntity.Property(e => e.ErrorType).IsRequired();
        failedProcessEntity.HasBaseType<ProcessDb>();
        var processRuntimeDataEntity = modelBuilder.Entity<ProcessRuntimeDataDb>();
        processRuntimeDataEntity.HasKey(e => e.Id);
        processRuntimeDataEntity.Property(e => e.Flow).IsRequired();
        processRuntimeDataEntity.Property(e => e.TotalQuantity).IsRequired();
        processRuntimeDataEntity.Property(e => e.TotalTime).IsRequired();
    }
}
