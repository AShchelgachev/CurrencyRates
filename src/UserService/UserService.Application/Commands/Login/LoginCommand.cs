using MediatR;
using UserService.Application.Dto;

namespace UserService.Application.Commands.Login;

public record LoginCommand(string Name, string Password) : IRequest<AccessToken?>;
