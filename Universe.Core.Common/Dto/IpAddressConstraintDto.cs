using Universe.Core.Common.Model;
using Universe.Core.Infrastructure;

namespace Universe.Core.Common.Dto;

public class IpAddressConstraintDto : IDto
{
	public string? AddressesRangeEnd { get; set; }

	public string AddressesRangeStart { get; set; } = string.Empty;

	public string? ContainingUrlPart { get; set; }

	public int Id { get; set; }

	public IpAddressConstraintRule Rule { get; set; }
}
