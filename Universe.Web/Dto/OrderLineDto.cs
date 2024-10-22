using Universe.Core.Infrastructure;
using Universe.Web.Model;

namespace Universe.Web.Dto;

public class OrderLineDto : IDto
{
	public int Count { get; set; }

	public int Id { get; set; }

	public OrderDto? Order { get; set; }

	public int OrderId { get; set; }

	public double Price { get; set; }

	public ProductDto? Product { get; set; }

	public int ProductId { get; set; }
}
