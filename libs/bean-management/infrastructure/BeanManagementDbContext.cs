using MicraPro.BeanManagement.Domain.StorageAccess;
using MicraPro.BeanManagement.Infrastructure.StorageAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MicraPro.BeanManagement.Infrastructure;

public class BeanManagementDbContext(IConfiguration? configuration = null) : DbContext
{
    public DbSet<RoasteryDb> RoasteryEntries { get; set; }
    public DbSet<BeanDb> BeanEntries { get; set; }
    public DbSet<RecipeDb> RecipeEntries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(configuration?.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var roasteryEntity = modelBuilder.Entity<RoasteryDb>();
        roasteryEntity.HasKey(e => e.Id);
        roasteryEntity.Property(e => e.Name).IsRequired();
        roasteryEntity.Property(e => e.Location).IsRequired();
        roasteryEntity
            .HasMany(e => e.Beans)
            .WithOne(e => e.RoasteryObject)
            .HasForeignKey(e => e.RoasteryId)
            .IsRequired();
        var beanEntity = modelBuilder.Entity<BeanDb>();
        beanEntity.HasKey(e => e.Id);
        beanEntity.Property(e => e.Name).IsRequired();
        beanEntity.Property(e => e.CountryCode).IsRequired();
        beanEntity
            .HasMany(e => e.Recipes)
            .WithOne(e => e.BeanObject)
            .HasForeignKey(e => e.BeanId)
            .IsRequired();
        var recipeEntity = modelBuilder.Entity<RecipeDb>();
        recipeEntity.HasKey(e => e.Id);
        var recipeEspressoEntity = modelBuilder.Entity<EspressoRecipeDb>();
        recipeEspressoEntity.Property(e => e.GrindSetting).IsRequired();
        recipeEspressoEntity.Property(e => e.CoffeeQuantity).IsRequired();
        recipeEspressoEntity.Property(e => e.InCupQuantity).IsRequired();
        recipeEspressoEntity.Property(e => e.BrewTemperature).IsRequired();
        recipeEspressoEntity.Property(e => e.TargetExtractionTime).IsRequired();
        recipeEspressoEntity.HasBaseType<RecipeDb>();
        var recipeV60Entity = modelBuilder.Entity<V60RecipeDb>();
        recipeV60Entity.Property(e => e.GrindSetting).IsRequired();
        recipeV60Entity.Property(e => e.CoffeeQuantity).IsRequired();
        recipeV60Entity.Property(e => e.InCupQuantity).IsRequired();
        recipeV60Entity.Property(e => e.BrewTemperature).IsRequired();
        recipeV60Entity.HasBaseType<RecipeDb>();
    }
}
