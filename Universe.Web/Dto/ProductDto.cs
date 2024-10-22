using Universe.Core.Infrastructure;

namespace Universe.Web.Dto;

public class ProductDto : IDto
{
	public int Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public double Price { get; set; }
}
