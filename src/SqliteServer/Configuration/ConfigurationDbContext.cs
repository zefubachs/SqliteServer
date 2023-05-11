using SqliteServer.Configuration.Entities;
using Microsoft.EntityFrameworkCore;

namespace SqliteServer.Configuration;
public class ConfigurationDbContext : DbContext
{
    public DbSet<Database> Databases => Set<Database>();

    public ConfigurationDbContext(DbContextOptions<ConfigurationDbContext> options) 
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("NOCASE");
        modelBuilder.Entity<Database>(e =>
        {
            e.ToTable("Database");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.HasIndex(x => x.Name).IsUnique();
        });
    }
}
