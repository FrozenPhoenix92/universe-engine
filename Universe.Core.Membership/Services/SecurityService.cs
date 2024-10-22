using Universe.Core.Data;
using Universe.Core.Exceptions;
using Universe.Core.Membership.Configuration;
using Universe.Core.Membership.Model;
using Universe.Core.Utils;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using System.Security.Cryptography;
using System.Text;

namespace Universe.Core.Membership.Services;

public class SecurityService : ISecurityService
{
	private readonly IDbContext _context;
	private readonly string _secureDataEncryptionKey;


	public SecurityService(IDbContext context, IOptions<SecuritySettings> securitySettingsOptions)
	{
		VariablesChecker.CheckIsNotNull(context, nameof(context));
		VariablesChecker.CheckIsNotNull(context, nameof(securitySettingsOptions));
		VariablesChecker.CheckIsNotNull(context, nameof(securitySettingsOptions.Value));
		VariablesChecker.CheckIsNotNullOrEmpty(
			securitySettingsOptions.Value.SecureDataEncryptionKey,
			nameof(securitySettingsOptions.Value.SecureDataEncryptionKey));

		_context = context;
		_secureDataEncryptionKey = securitySettingsOptions.Value.SecureDataEncryptionKey;
	}


	public async Task<string> EncryptString(string value, CancellationToken ct = default)
	{
		var initializationVector = Encoding.ASCII.GetBytes(_secureDataEncryptionKey);
		var initialSuperAdminData = await GetInitialSuperAdminEncryptionData(ct);

		using (Aes aes = Aes.Create())
		{
			aes.Key = Encoding.UTF8.GetBytes(initialSuperAdminData);
			aes.IV = initializationVector;
			var symmetricEncryptor = aes.CreateEncryptor(aes.Key, aes.IV);

			using (var memoryStream = new MemoryStream())
			{
				using (var cryptoStream = new CryptoStream(memoryStream, symmetricEncryptor, CryptoStreamMode.Write))
				{
					using (var streamWriter = new StreamWriter(cryptoStream))
					{
						streamWriter.Write(value);
					}

					return Convert.ToBase64String(memoryStream.ToArray());
				}
			}
		}
	}
	public async Task<string> DecryptString(string value, CancellationToken ct = default)
	{
		var initializationVector = Encoding.ASCII.GetBytes(_secureDataEncryptionKey);
		var initialSuperAdminData = await GetInitialSuperAdminEncryptionData(ct);
		var buffer = Convert.FromBase64String(value);

		using (Aes aes = Aes.Create())
		{
			aes.Key = Encoding.UTF8.GetBytes(initialSuperAdminData);
			aes.IV = initializationVector;
			var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

			using (var memoryStream = new MemoryStream(buffer))
			{
				using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
				{
					using (var streamReader = new StreamReader(cryptoStream))
					{
						return streamReader.ReadToEnd();
					}
				}
			}
		}
	}

	public async Task SavePasswordToHistory(User user, bool saveChanges = true, CancellationToken cancellationToken = default)
	{
		VariablesChecker.CheckIsNotNull(user, nameof(user));

		await _context.Set<PasswordChange>().AddAsync(new PasswordChange
		{
			UserId = user.Id,
			Password = user.PasswordHash,
			Created = DateTime.UtcNow,
		}, cancellationToken);

		if (saveChanges)
		{
			await _context.SaveChangesAsync(cancellationToken);
		}
	}


	private async Task<string> GetInitialSuperAdminEncryptionData(CancellationToken ct)
	{
		var firstRecord = await _context.Set<PasswordChange>().FirstOrDefaultAsync(ct);

		if (firstRecord is null)
			throw new ConflictException("База данных не содержит ни одной записи истории создания пароля, необходимой для шифрования.");

		return $"{firstRecord.UserId.ToString().Substring(0, 10)}{firstRecord.Password.Substring(10, 22)}";
	}
}
