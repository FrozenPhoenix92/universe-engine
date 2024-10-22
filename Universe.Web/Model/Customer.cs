using Universe.Core.Data;

namespace Universe.Web.Model;

public class Customer : IEntity
{
	public string Address { get; set; } = string.Empty;

	public int Id { get; set; }

	public string LastName { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	public string? Photo {  get; set; }
}
