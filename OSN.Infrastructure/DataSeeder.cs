using Bogus;
using OSN.Domain.Models;
using System.Security.Cryptography;
using System.Text;

namespace OSN.Infrastructure;

public class DataSeeder
{
    private readonly AppDbContext _db;
    public DataSeeder(AppDbContext db)
    {
        _db = db;
    }
    public void Seed()
    {
        Randomizer.Seed = new Random(0);
        var hashedPassword = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes("password")));
        var fakerUser = new Faker<User>()
            .RuleFor(u => u.Id, f => Guid.NewGuid())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PasswordHash, f => hashedPassword)
            .RuleFor(u => u.Role, f => f.PickRandom(new List<string> { "User", "Admin", "SuperAdmin" }))
            .RuleFor(u => u.IsDeleted, f => f.Random.Bool());

        var users = new List<User>();
        users = fakerUser.Generate(20);
        _db.Users.AddRange(users);
        _db.SaveChanges();
    }
}