using MediatR;
using OSN.Application.Repositories;
using OSN.Application.Services;
using OSN.Domain.Models;
using OSN.Domain.ValueObjects;
using OSN.Domain.Core;
using OSN.Application.Dto;

namespace OSN.Application.Features.Auth.GoogleSignIn;

public class GoogleSignInCommandHandler : IRequestHandler<GoogleSignInCommand, Result<GoogleSignInResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IGoogleSignInFieldsRepository _googleSignInFieldsRepository;
    private readonly IAuthService _authService;
    private readonly IGoogleOAuth2Service _googleOAuth2Service;

    public GoogleSignInCommandHandler(
        IUserRepository userRepository,
        IGoogleSignInFieldsRepository googleSignInFieldsRepository,
        IAuthService authService,
        IGoogleOAuth2Service googleOAuth2Service)
    {
        _userRepository = userRepository;
        _googleSignInFieldsRepository = googleSignInFieldsRepository;
        _authService = authService;
        _googleOAuth2Service = googleOAuth2Service;
    }

    public async Task<Result<GoogleSignInResponse>> Handle(GoogleSignInCommand command, CancellationToken ct)
    {
        string idToken;
        try
        {
            idToken = await _googleOAuth2Service.GetGoogleIdTokenAsync(command.AuthorizationCode, command.RedirectUri);
        }
        catch (HttpRequestException ex)
        {
            return Result<GoogleSignInResponse>.Failure($"Google token exchange failed: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            return Result<GoogleSignInResponse>.Failure($"Failed to retrieve ID token: {ex.Message}");
        }

        GoogleIdTokenDto payload;
        try
        {
            payload = await _googleOAuth2Service.ValidateIdTokenAsync(idToken);
        }
        catch (Exception ex)
        {
            return Result<GoogleSignInResponse>.Failure($"Error while validating Google Id Token: {ex.Message}");
        }


        var emailString = EmailString.Create(payload.Email);
        var existingUser = await _userRepository.GetUserByEmailAsync(emailString, ct);
        bool isNewUser = existingUser == null;

        
        if (isNewUser)
        {
            existingUser = new User
            {
                Id = Guid.NewGuid(),
                Email = emailString,
                PasswordHash = "",
                Role = RoleHierarchy.UserRole,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            var googleFields = new GoogleSignInFields
            {
                Id = Guid.NewGuid(),
                GoogleId = payload.GoogleId,
                FirstName = payload.FirstName,
                LastName = payload.LastName,
                ProfilePictureUrl = payload.ProfilePictureUrl,
                LinkedAt = DateTime.UtcNow,
                UserId = existingUser.Id
            };

            _userRepository.Add(existingUser);
            _googleSignInFieldsRepository.Add(googleFields);
            await _userRepository.UnitOfWork.SaveChangesAsync(ct);

        }
        // User registered with email but not linked to Google
        else if (existingUser!.GoogleSignIn == null)
        {
            var googleFields = new GoogleSignInFields
            {
                Id = Guid.NewGuid(),
                GoogleId = payload.GoogleId,
                FirstName = payload.FirstName,
                LastName = payload.LastName,
                ProfilePictureUrl = payload.ProfilePictureUrl,
                LinkedAt = DateTime.UtcNow,
                UserId = existingUser.Id
            };

            _googleSignInFieldsRepository.Add(googleFields);
            await _userRepository.UnitOfWork.SaveChangesAsync(ct);
        }
        // update existing Google sign-in fields
        else
        {
            existingUser.GoogleSignIn.FirstName = payload.FirstName;
            existingUser.GoogleSignIn.LastName = payload.LastName;
            existingUser.GoogleSignIn.ProfilePictureUrl = payload.ProfilePictureUrl;
            _googleSignInFieldsRepository.Update(existingUser.GoogleSignIn);
            await _userRepository.UnitOfWork.SaveChangesAsync(ct);
        }

        var token = _authService.GenearateJwtToken(existingUser);

        return Result<GoogleSignInResponse>.Success(new GoogleSignInResponse(token, existingUser.Role, isNewUser));
    }
}