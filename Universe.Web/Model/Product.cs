using Universe.Core.Data;

namespace Universe.Web.Model;

public class Product : IEntity
{
	public int Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public double Price { get; set; }
}
