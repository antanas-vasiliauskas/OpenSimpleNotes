using Microsoft.EntityFrameworkCore;
using OSN.Domain.Daos;

namespace OSN.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<UserDao> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserDao>(x =>
        {
            x.HasKey(u => u.Id);
        });
    }
}