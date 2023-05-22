using SqliteServer.Data.Tracing.Entities;

namespace SqliteServer.Data.Tracing;

public class TracingDbContext : DbContext
{
    public DbSet<CommandLog> Logs => Set<CommandLog>();

    public TracingDbContext(DbContextOptions<TracingDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CommandLog>(e =>
        {
            e.ToTable("CommandLog");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.User);
            e.HasIndex(x => x.Database);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.Database).IsRequired().HasMaxLength(200);
        });
    }
}
