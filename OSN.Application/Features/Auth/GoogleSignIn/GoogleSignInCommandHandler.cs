using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth;
using OSN.Domain.Models;
using OSN.Infrastructure;
using OSN.Infrastructure.Services;

namespace OSN.Application.Features.Auth.GoogleSignIn;

public class GoogleSignInCommandHandler : IRequestHandler<GoogleSignInCommand, Result<GoogleSignInResponse>>
{
    private readonly AppDbContext _db;
    private readonly AuthService _authService;
    private readonly GoogleOAuth2Service _googleOAuth2Service;
    private readonly IConfiguration _configuration;

    public GoogleSignInCommandHandler(AppDbContext db, AuthService authService, GoogleOAuth2Service googleOAuth2Service, IConfiguration configuration)
    {
        _db = db;
        _authService = authService;
        _googleOAuth2Service = googleOAuth2Service;
        _configuration = configuration;
    }

    public async Task<Result<GoogleSignInResponse>> Handle(GoogleSignInCommand command, CancellationToken ct)
    {
        try
        {
            var request = command.Request;

            // Exchange authorization code for ID token
            var idToken = await _googleOAuth2Service.ExchangeAuthorizationCodeAsync(request.AuthorizationCode, request.RedirectUri);

            if (string.IsNullOrEmpty(idToken))
            {
                return Result<GoogleSignInResponse>.Failure("Failed to obtain ID token from Google.");
            }

            // Validate the ID token
            var clientId = _configuration["Authentication:Google:ClientId"];
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { clientId }
            });

            if (payload == null)
            {
                return Result<GoogleSignInResponse>.Failure("Invalid Google ID token.");
            }

            var existingUser = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == payload.Email, ct);
            
            bool isNewUser = false;

            if (existingUser == null)
            {
                existingUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = payload.Email,
                    PasswordHash = "", // No password for Google users
                    Role = RoleHierarchy.UserRole,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                var googleFields = new GoogleSignInFields
                {
                    Id = Guid.NewGuid(),
                    GoogleId = payload.Subject,
                    FirstName = payload.GivenName ?? "",
                    LastName = payload.FamilyName ?? "",
                    ProfilePictureUrl = payload.Picture ?? "",
                    LinkedAt = DateTime.UtcNow,
                    UserId = existingUser.Id
                };

                _db.Users.Add(existingUser);
                _db.GoogleSignInFields.Add(googleFields);
                await _db.SaveChangesAsync(ct);
                isNewUser = true;
            }
            else if (existingUser.GoogleSignIn == null)
            {
                // Existing user linking Google account for the first time
                var googleFields = new GoogleSignInFields
                {
                    Id = Guid.NewGuid(),
                    GoogleId = payload.Subject,
                    FirstName = payload.GivenName ?? "",
                    LastName = payload.FamilyName ?? "",
                    ProfilePictureUrl = payload.Picture ?? "",
                    LinkedAt = DateTime.UtcNow,
                    UserId = existingUser.Id
                };

                _db.GoogleSignInFields.Add(googleFields);
                await _db.SaveChangesAsync(ct);
            }
            else
            {
                // Update existing Google fields with latest info
                existingUser.GoogleSignIn.FirstName = payload.GivenName ?? existingUser.GoogleSignIn.FirstName;
                existingUser.GoogleSignIn.LastName = payload.FamilyName ?? existingUser.GoogleSignIn.LastName;
                existingUser.GoogleSignIn.ProfilePictureUrl = payload.Picture ?? existingUser.GoogleSignIn.ProfilePictureUrl;
                await _db.SaveChangesAsync(ct);
            }

            var token = _authService.GenearateToken(existingUser);

            return Result<GoogleSignInResponse>.Success(new GoogleSignInResponse(token, existingUser.Role, isNewUser));
        }
        catch (HttpRequestException ex)
        {
            return Result<GoogleSignInResponse>.Failure($"Google token exchange failed: {ex.Message}");
        }
        catch (InvalidJwtException)
        {
            return Result<GoogleSignInResponse>.Failure("Invalid Google ID token.");
        }
        catch (Exception ex)
        {
            return Result<GoogleSignInResponse>.Failure($"Google sign-in failed: {ex.Message}");
        }
    }
}