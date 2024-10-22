using Universe.Core.Infrastructure;
using Universe.Core.QueryHandling;
using Universe.Web.Dto;
using Universe.Web.Model;

namespace Universe.Web.Services;

public interface IOrderService : IChangeQueryDataOperation<Order>
{
	Task<double> GetOrdersSum(QueryCommand command, CancellationToken ct = default);

	Task<ProductFrequency?> GetMostFrequentProduct(QueryCommand command, CancellationToken ct = default);
}
