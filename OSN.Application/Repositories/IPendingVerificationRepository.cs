using OSN.Domain.Models;
using OSN.Domain.ValueObjects;

namespace OSN.Application.Repositories;

public interface IPendingVerificationRepository: IRepository
{
    Task<PendingVerification?> GetByEmailAsync(EmailString email, CancellationToken cancellationToken = default);
    void Add(PendingVerification pendingVerification);
    void Update(PendingVerification pendingVerification);
    void Remove(PendingVerification pendingVerification);
}