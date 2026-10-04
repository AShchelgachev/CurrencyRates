using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using UserService.Application.Commands.Login;
using UserService.Application.Commands.Logout;
using UserService.Application.Commands.Register;
using UserService.Application.Dto;

namespace UserService.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        if (id is null)
        {
            return Conflict();
        }

        return new RegisterResponse(id.Value);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AccessToken>> Login(LoginCommand command, CancellationToken cancellationToken)
    {
        var token = await _mediator.Send(command, cancellationToken);
        if (token is null)
        {
            return Unauthorized();
        }

        return token;
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti)!;
        var exp = long.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Exp)!);
        var expiresAt = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;

        await _mediator.Send(new LogoutCommand(jti, expiresAt), cancellationToken);
        return NoContent();
    }

    public record RegisterResponse(int Id);
}
