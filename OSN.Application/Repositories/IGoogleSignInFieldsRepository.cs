using OSN.Domain.Models;

namespace OSN.Application.Repositories;

public interface IGoogleSignInFieldsRepository : IRepository
{
    void Add(GoogleSignInFields googleSignInFields);
    void Update(GoogleSignInFields googleSignInFields);
}