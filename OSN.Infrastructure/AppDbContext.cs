using Microsoft.EntityFrameworkCore;
using OSN.Domain.Models;

namespace OSN.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<GoogleSignInFields> GoogleSignInFields { get; set; }
    public DbSet<PendingVerification> PendingVerifications { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeToUtcConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(x =>
        {
            x.HasKey(u => u.Id);

            x.HasIndex(u => u.Email).IsUnique();

            x.Navigation(u => u.Notes)
                .AutoInclude();

            x.Navigation(u => u.GoogleSignIn)
                .AutoInclude();
        });

        modelBuilder.Entity<Note>(x =>
        {
            x.HasKey(u => u.Id);

            x.HasOne(n => n.User)
                .WithMany(u => u.Notes)
                .HasForeignKey(n => n.UserId)
                .IsRequired();

            x.Navigation(n => n.User)
                .AutoInclude();
        });

        modelBuilder.Entity<GoogleSignInFields>(x =>
        {
            x.HasKey(g => g.Id);

            x.HasIndex(g => g.GoogleId).IsUnique();

            x.HasOne<User>()
                .WithOne(u => u.GoogleSignIn)
                .HasForeignKey<GoogleSignInFields>(g => g.UserId)
                .IsRequired();
        });

        modelBuilder.Entity<PendingVerification>(x =>
        {
            x.HasKey(p => p.Id);

            x.HasIndex(p => p.Email).IsUnique();
        });
    }
}
