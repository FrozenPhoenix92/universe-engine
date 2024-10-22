using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Universe.Web.Utils;

public class PasswordHelper
{
	public static byte[] GetSecureSalt() => RandomNumberGenerator.GetBytes(32);


	public static string HashUsingPbkdf2(string password, byte[] salt)
	{
		byte[] derivedKey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, iterationCount: 300000, 32);
		return Convert.ToBase64String(derivedKey);
	}
}
