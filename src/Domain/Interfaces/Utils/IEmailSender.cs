namespace Domain.Interfaces.Utils;

/// <summary>
/// Email sender inteface
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Send email to user with the token, to recover password
    /// </summary>
    /// <param name="Email">Email of the user</param>
    /// <param name="token">Auth token</param>
    /// <returns>A asyncronous operation</returns>
    public Task SendRecoverPasswordAsync(string Email, string token);
}
