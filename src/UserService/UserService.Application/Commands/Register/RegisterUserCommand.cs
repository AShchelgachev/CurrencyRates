using System.ComponentModel.DataAnnotations;
using MediatR;

namespace UserService.Application.Commands.Register;

public record RegisterUserCommand(
    [Required, MaxLength(100)] string Name,
    [Required, MinLength(6), MaxLength(100)] string Password) : IRequest<int?>;
