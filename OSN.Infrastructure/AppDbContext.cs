using Microsoft.EntityFrameworkCore;
using OSN.Application.Repositories;
using OSN.Domain.Models;
using OSN.Domain.ValueObjects;
using OSN.Infrastructure.ValueConverters;

namespace OSN.Infrastructure;

public class AppDbContext : DbContext, IUnitOfWork
{
    // migration commands:
    // dotnet ef migrations add <MigrationName> --project ..\OSN.Infrastructure\OSN.Infrastructure.csproj --startup-project .\OSN.csproj
    // dotnet ef database update
    // dotnet ef database update --connection "your-remote-connection-string"
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
        configurationBuilder.Properties<EmailString>().HaveConversion<EmailStringConverter>();
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
