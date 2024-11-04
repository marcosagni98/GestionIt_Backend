using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    /// <summary>
    /// Returns true if the email exists in the database
    /// </summary>
    /// <param name="email">The email of the user </param>
    /// <returns>A bool value that represents if the user exists or not</returns>
    public Task<bool> EmailExistsAsync(string email);

    /// <summary>
    /// Returns if there is a user with the email and password given
    /// </summary>
    /// <param name="email">Email of the user</param>
    /// <param name="password">Password of the user</param>
    /// <returns>True if the user exists, false otherwise</returns>
    public Task<bool> LoginAsync(string email, string password);

    /// <summary>
    /// Returns the user with the email given
    /// </summary>
    /// <param name="email">Email of the user</param>
    /// <returns>User entity</returns>
    public Task<User?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Returns a list of technicians
    /// </summary>
    /// <returns>List of User entities who are technicians</returns>
    public Task<List<User?>> GetAllTechniantsAsync();
}
