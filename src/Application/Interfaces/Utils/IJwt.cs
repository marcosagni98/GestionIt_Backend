using Application.Dtos.Auth.Response;
using Domain.Entities;

namespace Application.Interfaces.Utils;

/// <summary>
/// Jwt related operations
/// </summary>
public interface IJwt
{
    /// <summary>
    /// Generate jwt token
    /// </summary>
    /// <param name="user">User data</param>
    /// <returns>a <see cref="LoginResponseDto"/> with the token and the time </returns>
    public LoginResponseDto GenerateJwtToken(User user);

}
