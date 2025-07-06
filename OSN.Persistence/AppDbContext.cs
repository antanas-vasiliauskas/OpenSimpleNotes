using Microsoft.EntityFrameworkCore;
using OSN.Domain.Daos;

namespace OSN.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<UserDao> Users { get; set; }
    public DbSet<NoteDao> Notes { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserDao>(x =>
        {
            x.HasKey(u => u.Id);

            x.HasMany<NoteDao>(u => u.Notes)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<NoteDao>(x =>
        {
            x.HasKey(n => n.Id);

            x.HasOne<UserDao>(n => n.User)
                .WithMany(u => u.Notes)
                .HasForeignKey(n => n.UserId)
                .IsRequired();
        });
    }
}