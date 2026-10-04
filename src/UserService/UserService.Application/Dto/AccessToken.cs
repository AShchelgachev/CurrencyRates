namespace UserService.Application.Dto;

public record AccessToken(string Token, DateTime ExpiresAt);
