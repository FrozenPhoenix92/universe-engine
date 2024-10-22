using Universe.Core.Infrastructure;
using Universe.Web.Model;

namespace Universe.Web.Dto;

public class OrderDto : IDto
{
	public DateTimeOffset Created { get; set; }

	public CustomerDto? Customer { get; set; }

	public int CustomerId { get; set; }

	public int Id { get; set; }

	public DateTimeOffset LastModified { get; set; }

	public IEnumerable<OrderLineDto> OrderLines { get; set; } = new List<OrderLineDto>();

	public OrderStatus Status { get; set; }
}
