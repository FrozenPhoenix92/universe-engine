using Universe.Data;
using Universe.Web.Model;

using Microsoft.EntityFrameworkCore;

namespace Universe.Web.Services;

public class ClientOperationConfirmationTokenService : IClientOperationConfirmationTokenService
{
	public const string EmailConfirmationOperationName = "EmailConfirmation";
	public const string PasswordRecoveryOperationName = "PasswordRecovery";

	private readonly IDataContext _dataContext;


	public ClientOperationConfirmationTokenService(IDataContext dataContext) => _dataContext = dataContext;


	public async Task DeleteToken(Guid clientId, string operation, CancellationToken ct = default)
	{
		var token = await _dataContext.Set<ClientOperationConfirmationToken>()
			.SingleOrDefaultAsync(x => x.ClientId == clientId && x.Operation == operation, ct);

		if (token == null) return;

		_dataContext.Entry(token).State = EntityState.Deleted;

		await _dataContext.SaveChangesAsync(ct);
	}
	
	public async Task<ClientOperationConfirmationToken> GenerateToken(Guid clientId, string operation, int minutesLifetime = 0, CancellationToken ct = default)
	{
		var token = await _dataContext.Set<ClientOperationConfirmationToken>()
			.SingleOrDefaultAsync(x => x.ClientId == clientId && x.Operation == operation, ct)
			?? new ClientOperationConfirmationToken
			{
				ClientId = clientId,
				Operation = operation
			};

		token.ExpiredAt = minutesLifetime <= 0 ? null : DateTime.UtcNow.AddMinutes(minutesLifetime);
		token.Token = Guid.NewGuid().ToString();
		token.UpdatedAt = DateTime.UtcNow;

		if (token.Id == default)
		{
			await _dataContext.Set<ClientOperationConfirmationToken>().AddAsync(token, ct);
		}

		await _dataContext.SaveChangesAsync(ct);

		return token;
	}

	public async Task<ClientOperationConfirmationToken?> GetToken(Guid clientId, string operation, CancellationToken ct = default) 
		=> await _dataContext.Set<ClientOperationConfirmationToken>().SingleOrDefaultAsync(x => x.ClientId == clientId && x.Operation == operation, ct);

	public async Task<bool> ValidateTokenValue(string token, Guid clientId, string operation, CancellationToken ct = default)
	{
		var tokenEntity = await _dataContext.Set<ClientOperationConfirmationToken>()
			.SingleOrDefaultAsync(x => x.ClientId == clientId && x.Operation == operation, ct);

		return tokenEntity != null && (tokenEntity.ExpiredAt == null || tokenEntity.ExpiredAt >= DateTime.UtcNow) && tokenEntity.Token == token;
	}
}
