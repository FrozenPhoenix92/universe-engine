using Universe.Web.Model;

namespace Universe.Web.Services;

public interface IClientOperationConfirmationTokenService
{
	Task DeleteToken(Guid customerId, string operation, CancellationToken ct = default);

	Task<ClientOperationConfirmationToken> GenerateToken(Guid customerId, string operation, int minutesLifetime = 0, CancellationToken ct = default);

	Task<ClientOperationConfirmationToken?> GetToken(Guid customerId, string operation, CancellationToken ct = default);

	Task<bool> ValidateTokenValue(string token, Guid customerId, string operation, CancellationToken ct = default);
}
