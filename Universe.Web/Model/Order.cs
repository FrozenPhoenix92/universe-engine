using System.Text.Json.Serialization;

using Universe.Core.Data;
using Universe.Core.JsonSerialization;

namespace Universe.Web.Model;

[JsonConverter(typeof(JsonStringEnumCamelCaseConverter))]
public enum OrderStatus
{
	Created = 0,
	InProgress = 1,
	Completed = 2,
	Canceled = 3
}

public class Order : IEntity
{
	public DateTimeOffset Created {  get; set; }

	public Customer? Customer { get; set; }

	public int CustomerId { get; set; }

	public int Id { get; set; }

	public DateTimeOffset LastModified { get; set; }

	public IEnumerable<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

	public OrderStatus Status { get; set; }
}
