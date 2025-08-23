using Microsoft.EntityFrameworkCore;
using OSN.Application.Repositories;
using OSN.Domain.Core;
using OSN.Domain.Models;
using OSN.Domain.ValueObjects;

namespace OSN.Infrastructure.Repositories;

public class PendingVerificationRepository : IPendingVerificationRepository
{
    private readonly AppDbContext _db;
    public IUnitOfWork UnitOfWork => _db; // just expose save async

    public PendingVerificationRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PendingVerification?> GetByEmailAsync(EmailString email, CancellationToken cancellationToken = default)
    {
        return await _db.PendingVerifications.FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
    }

    public void Add(PendingVerification pendingVerification)
    {
        _db.PendingVerifications.Add(pendingVerification); 
    }

    public void Update(PendingVerification pendingVerification)
    {
        _db.PendingVerifications.Update(pendingVerification);
    }

    public void Remove(PendingVerification pendingVerification)
    {
        _db.PendingVerifications.Remove(pendingVerification);
    }
}