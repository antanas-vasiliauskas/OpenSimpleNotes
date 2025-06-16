using Bogus;
using OSN.Domain.Daos;
using OSN.Domain.Enums;
using System.Security.Cryptography;
using System.Text;

namespace OSN.Persistence;

public class DataSeeder
{
    private readonly AppDbContext _context;

    public DataSeeder(AppDbContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        Randomizer.Seed = new Random(0);
        var hashedPassword = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes("password")));
        var fakerUser = new Faker<UserDao>()
                        .RuleFor(u => u.Id, f => Guid.NewGuid())
                        .RuleFor(u => u.Name, f => f.Name.FullName())
                        .RuleFor(u => u.Email, f => f.Internet.Email())
                        .RuleFor(u => u.Password, f => hashedPassword)
                        .RuleFor(u => u.Role, f => f.PickRandom<UserRole>())
                        .RuleFor(u => u.IsDeleted, f => f.Random.Bool());
        var users = new List<UserDao>();
        users = fakerUser.Generate(15);
        _context.Users.AddRange(users);
        _context.SaveChanges();
    }
}