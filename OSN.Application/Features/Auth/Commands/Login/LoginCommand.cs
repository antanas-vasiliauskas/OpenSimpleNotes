using MediatR;
using OSN.Application.Interfaces.Markers;
using OSN.Application.Models.Requests.Auth;
using OSN.Application.Models.Responses.Auth;

namespace OSN.Application.Features.Auth.Commands.Login;

public record LoginCommand(LoginRequest Request) : IRequest<LoginResponse>, ISkipAuthentication;