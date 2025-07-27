using Microsoft.EntityFrameworkCore;
using OSN.Domain.Models;

namespace OSN.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(x =>
        {
            x.HasKey(u => u.Id);
            x.HasIndex(u => u.Email).IsUnique();
        });
    }
}
