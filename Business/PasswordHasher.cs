using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Business;

public static class PasswordHasher
{
    public static string Hash(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hashBytes);
    }

    public static bool Verify(string password, string storedHash)
    {
        var hashOfInput = Hash(password);
        return hashOfInput == storedHash;
    }
}

