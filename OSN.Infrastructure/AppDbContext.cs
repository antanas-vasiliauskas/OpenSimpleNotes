using Microsoft.EntityFrameworkCore;
using OSN.Domain.Models;

namespace OSN.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Note> Notes { get; set; }

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
    }
}
