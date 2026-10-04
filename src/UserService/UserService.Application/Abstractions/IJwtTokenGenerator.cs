using UserService.Application.Dto;
using UserService.Domain.Entities;

namespace UserService.Application.Abstractions;

public interface IJwtTokenGenerator
{
    AccessToken Generate(User user);
}
