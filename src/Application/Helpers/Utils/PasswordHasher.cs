using System.Security.Cryptography;


namespace Application.Helpers.Utils;

/// <summary>
/// Password hashing utility class.
/// </summary>
public class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 20;
    private const int Iterations = 10000;

    /// <summary>
    /// Hashes the provided password using the PBKDF2 algorithm with a random salt.
    /// </summary>
    /// <param name="password">The plaintext password to be hashed.</param>
    /// <returns>A string containing the hashed password and salt.</returns>
    public static string HashPassword(string password)
    {
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        var hash = CreateHash(password, salt);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    /// <summary>
    /// Verifies if the provided password matches the hashed password.
    /// </summary>
    /// <param name="password">The plaintext password to be verified.</param>
    /// <param name="hashedPassword">The stored hashed password and salt.</param>
    /// <returns>True if the password is valid, false otherwise.</returns>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split('.');
        if (parts.Length != 2)
            return false;
        var salt = Convert.FromBase64String(parts[0]);
        var hash = Convert.FromBase64String(parts[1]);
        var newHash = CreateHash(password, salt);
        return SlowEquals(hash, newHash);
    }

    /// <summary>
    /// Creates a password hash using the PBKDF2 algorithm with the provided password and salt.
    /// </summary>
    /// <param name="password">The plaintext password to be hashed.</param>
    /// <param name="salt">The salt used to hash the password.</param>
    /// <returns>The hashed password.</returns>
    private static byte[] CreateHash(string password, byte[] salt)
    {
        using var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations);
        return deriveBytes.GetBytes(HashSize);
    }

    /// <summary>
    /// Compares two byte arrays in constant time to prevent timing attacks.
    /// </summary>
    /// <param name="a">The first byte array to compare.</param>
    /// <param name="b">The second byte array to compare.</param>
    /// <returns>True if the byte arrays are equal, false otherwise.</returns>
    private static bool SlowEquals(byte[] a, byte[] b)
    {
        uint diff = (uint)a.Length ^ (uint)b.Length;
        for (int i = 0; i < Math.Min(a.Length, b.Length); i++)
            diff |= (uint)(a[i] ^ b[i]);
        return diff == 0;
    }
}
