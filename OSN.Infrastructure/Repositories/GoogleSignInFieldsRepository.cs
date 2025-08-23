using OSN.Application.Repositories;
using OSN.Domain.Core;
using OSN.Domain.Models;

namespace OSN.Infrastructure.Repositories;

public class GoogleSignInFieldsRepository : IGoogleSignInFieldsRepository
{
    private readonly AppDbContext _db;
    public IUnitOfWork UnitOfWork => _db; // just expose save async

    public GoogleSignInFieldsRepository(AppDbContext db)
    {
        _db = db;
    }

    public void Add(GoogleSignInFields googleSignInFields)
    {
        _db.GoogleSignInFields.Add(googleSignInFields);
    }

    public void Update(GoogleSignInFields googleSignInFields)
    {
        _db.GoogleSignInFields.Update(googleSignInFields);
    }
}