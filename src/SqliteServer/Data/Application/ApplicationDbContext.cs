using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SqliteServer.Data.Application.Entities;

namespace SqliteServer.Data.Configuration;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public DbSet<Database> Databases => Set<Database>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Database>(e =>
        {
            e.ToTable("Database");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200).UseCollation("NOCASE");
            e.HasIndex(x => x.Name).IsUnique();
        });

        builder.Entity<ApplicationUser>(e =>
        {
            e.ToTable("ApplicationUser");
        });
        builder.Entity<IdentityUserClaim<Guid>>(e =>
        {
            e.ToTable("ApplicationUserClaim");
        });
        builder.Entity<IdentityUserToken<Guid>>(e =>
        {
            e.ToTable("ApplicationUserToken");
        });
        builder.Entity<IdentityUserClaim<Guid>>(e =>
        {
            e.ToTable("ApplicationUserClaim");
        });
        builder.Entity<IdentityUserLogin<Guid>>(e =>
        {
            e.ToTable("ApplicationUserLogin");
        });
        builder.Entity<ApplicationRole>(e =>
        {
            e.ToTable("ApplicationRole");
        });
        builder.Entity<IdentityUserRole<Guid>>(e =>
        {
            e.ToTable("ApplicationUserRole");
        });
        builder.Entity<IdentityRoleClaim<Guid>>(e =>
        {
            e.ToTable("ApplicationRoleClaim");
        });
    }
}
