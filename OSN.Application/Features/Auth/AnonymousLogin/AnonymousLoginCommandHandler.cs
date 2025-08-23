using MediatR;
using OSN.Application.Repositories;
using OSN.Application.Services;
using OSN.Domain.Core;
using OSN.Domain.Models;

namespace OSN.Application.Features.Auth.AnonymousLogin;

public class AnonymousLoginCommandHandler : IRequestHandler<AnonymousLoginCommand, Result<AnonymousLoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public AnonymousLoginCommandHandler(IUserRepository userRepository, IAuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<Result<AnonymousLoginResponse>> Handle(AnonymousLoginCommand command, CancellationToken ct)
    {
        User? user = null;
        bool isNewUser = false;

        if (command.GuestId != null)
        {
            user = await _userRepository.GetUserByIdAsync(command.GuestId.Value, ct);
        }

        if(user == null)
        {
            // if guestId is null or user not found, create a new guest user
            isNewUser = true;
            user = new User
            {
                Id = Guid.NewGuid(),
                Email = null,
                PasswordHash = string.Empty,
                Role = RoleHierarchy.GuestRole,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };
            _userRepository.Add(user);
            await _userRepository.UnitOfWork.SaveChangesAsync(ct);
        }

        var token = _authService.GenearateJwtToken(user);
        return Result<AnonymousLoginResponse>.Success(new AnonymousLoginResponse(token, user.Role, user.Id, isNewUser));

    }
}