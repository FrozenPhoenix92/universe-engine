using Universe.Core.Infrastructure;

namespace Universe.Web.Dto;

public class CustomerDto : IDto
{
	public string Address { get; set; } = string.Empty;

	public int Id { get; set; }

	public string LastName { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	public string? Photo { get; set; }
}
