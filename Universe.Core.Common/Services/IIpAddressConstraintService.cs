using Universe.Core.Common.Dto;
using Universe.Core.Common.Model;
using Universe.Core.Infrastructure;

namespace Universe.Core.Common.Services;

public interface IIpAddressConstraintService : IValidateDataOperation<IpAddressConstraintDto, IpAddressConstraint>
{
	Task<bool> IsAllowedRequest(string ipAddress, string url, CancellationToken ct = default);
}
