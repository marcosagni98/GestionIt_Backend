using Application.Dtos.Auth.Response;
using Application.Interfaces.Utils;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers.Utils;

/// <summary>
/// Jwt related operations
/// </summary>
public class Jwt : IJwt
{
    private int _expirationTimeSec = 86400;

    /// <inheritdoc/>
    public LoginResponseDto GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        DateTime expirationDate = DateTime.UtcNow.AddSeconds(_expirationTimeSec);
        var key = Encoding.ASCII.GetBytes("supersecretkeysupersecretkeysupersecretkey");
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
        List<Claim> claims =
        [
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, ((int)user.UserType).ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        ];

        var tokenDescriptor = new JwtSecurityToken
        (
            issuer: "gestionIt_api",
            claims: claims,
            expires: expirationDate,
            signingCredentials: signingCredentials,
            audience: "gestionIt_frontend"
        );

        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        return new LoginResponseDto(token, expirationDate);
    }
}
