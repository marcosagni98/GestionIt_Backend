using Application.Dtos.Auth.Response;
using Application.Interfaces.Utils;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utils;

/// <summary>
/// Jwt related operations
/// </summary>
public class Jwt(IConfiguration configuration) : IJwt
{
    private readonly IConfiguration _configuration = configuration;
    private readonly int _expirationTimeSec = 86400;

    /// <inheritdoc/>
    public LoginResponseDto GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        DateTime expirationDate = DateTime.UtcNow.AddSeconds(_expirationTimeSec);
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
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
            issuer: jwtSettings["Issuer"],
            claims: claims,
            expires: expirationDate,
            signingCredentials: signingCredentials,
            audience: jwtSettings["Audience"]
        );

        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        return new LoginResponseDto(token, expirationDate);
    }
}
