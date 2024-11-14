namespace Application.Dtos.Auth.Requests;

/// <summary>
/// Represents a login request containing the user's email and password.
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
public record LoginRequestDto(
    string Email,
    string Password
);
