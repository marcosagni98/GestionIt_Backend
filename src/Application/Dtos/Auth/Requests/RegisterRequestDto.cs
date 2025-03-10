﻿using Domain.Enums;
using System.Text.Json.Serialization;

namespace Application.Dtos.Auth.Requests;

/// <summary>
/// register new user request dto
/// </summary>
public class RegisterRequestDto
{
    /// <summary>
    /// Full name of the user
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Email of the user
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Password of the user
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// User type
    /// </summary>
    [JsonIgnore]
    public UserType UserType { get; set; } = UserType.Basic;
}
