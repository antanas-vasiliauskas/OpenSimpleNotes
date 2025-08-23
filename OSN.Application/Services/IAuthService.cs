using OSN.Domain.Models;

namespace OSN.Application.Services;

public interface IAuthService
{
    string GenearateJwtToken(User user);
}