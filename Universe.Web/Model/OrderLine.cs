using Universe.Core.Data;

namespace Universe.Web.Model;

public class OrderLine : IEntity
{
	public int Count { get; set; }

	public int Id { get; set; }

	public Order? Order { get; set; }

	public int OrderId { get; set; }

	public double Price { get; set; }

	public Product? Product { get; set; }

	public int ProductId { get; set; }
}
