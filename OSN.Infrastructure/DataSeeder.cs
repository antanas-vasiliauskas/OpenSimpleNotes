using Bogus;
using OSN.Domain.Models;
using OSN.Infrastructure.Services;

namespace OSN.Infrastructure;

public class DataSeeder
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher _passwordHasher;
    public DataSeeder(AppDbContext db, PasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }
    public void Seed()
    {
        Randomizer.Seed = new Random(0);
        var hashedPassword = _passwordHasher.HashSHA256Password("password");
        var fakerUser = new Faker<User>()
            .RuleFor(u => u.Id, f => Guid.NewGuid())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PasswordHash, f => hashedPassword)
            .RuleFor(u => u.Role, f => f.PickRandom(new List<string> { "User", "Admin", "SuperAdmin" }))
            .RuleFor(u => u.IsDeleted, f => f.Random.Bool())
            .RuleFor(n => n.CreatedAt, f => DateTime.Now);

        var users = new List<User>();
        users = fakerUser.Generate(20);
        _db.Users.AddRange(users);
        _db.SaveChanges();

        var notes = new List<Note>();
        var fakerNote = new Faker<Note>()
            .RuleFor(n => n.Id, f => Guid.NewGuid())
            .RuleFor(n => n.Title, f => string.Join(" ", f.Lorem.Words(f.Random.Int(1, 3))))
            .RuleFor(n => n.Content, f => string.Join(" ", f.Lorem.Words(f.Random.Int(20, 500))))
            .RuleFor(n => n.IsDeleted, f => f.Random.Bool())
            .RuleFor(n => n.IsPinned, f => f.Random.Bool())
            .RuleFor(n => n.CreatedAt, f => DateTime.Now)
            .RuleFor(n => n.UpdatedAt, f => DateTime.Now);

        foreach(var user in users)
        {
            var userNotes = fakerNote.Generate(5);
            foreach(var note in userNotes)
            {
                note.UserId = user.Id;
            }
            notes.AddRange(userNotes);
        }
        _db.Notes.AddRange(notes);
        _db.SaveChanges();
    }
}