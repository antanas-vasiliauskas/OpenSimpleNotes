using MediatR;

namespace OSN.Application.Features.Auth.Register;
public record RegisterCommand(string Email, string Password) : IRequest<Result<RegisterResponse>>;