using MediatR;

namespace UserService.Application.Commands.Logout;

public record LogoutCommand(string Jti, DateTime ExpiresAt) : IRequest;
