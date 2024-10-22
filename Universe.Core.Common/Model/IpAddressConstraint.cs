using Universe.Core.Data;

namespace Universe.Core.Common.Model;


public enum IpAddressConstraintRule
{
	Allow = 0,
	Forbid = 1
}

public class IpAddressConstraint : IEntity
{
	public string? AddressesRangeEnd { get; set; }

	public string AddressesRangeStart { get; set; } = string.Empty;

	public string? ContainingUrlPart { get; set; }
	
	public int Id { get; set; }

	public IpAddressConstraintRule Rule { get; set; }
}
