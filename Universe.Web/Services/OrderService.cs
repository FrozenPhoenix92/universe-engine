using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Universe.Core.QueryHandling;
using Universe.Data;
using Universe.Web.Dto;
using Universe.Web.Model;


namespace Universe.Web.Services;

public class OrderService : IOrderService
{
	private readonly IDataContext _dataContext;
	private readonly IMapper _mapper;


	public OrderService(IMapper mapper, IDataContext dataContext)
	{
		_mapper = mapper;
		_dataContext = dataContext;
	}


	public async Task<ProductFrequency?> GetMostFrequentProduct(QueryCommand command, CancellationToken ct = default)
	{
		var mostFrequentItem = await ApplyQueryCommand(_dataContext.Set<Order>().Include(x => x.OrderLines).ThenInclude(x => x.Product), command)
				.SelectMany(x => x.OrderLines)
				.GroupBy(x => x.ProductId)
				.Select(x => new { x.First().Product, Count = x.Sum(y => y.Count) })
				.OrderByDescending(x => x.Count)
				.FirstOrDefaultAsync(ct);

		return mostFrequentItem == null
			? null
			: new ProductFrequency 
			{
				Product = _mapper.Map<ProductDto>(mostFrequentItem.Product),
				Count = mostFrequentItem.Count
			};
}

	public async Task<double> GetOrdersSum(QueryCommand command, CancellationToken ct = default)
		=> await ApplyQueryCommand(_dataContext.Set<Order>().Include(x => x.OrderLines), command)
			.SelectMany(x => x.OrderLines)
			.SumAsync(x => x.Price, ct);

	public Func<IQueryable<Order>, IQueryable<Order>> GetQueryHandler()
		=> queryable => queryable.Include(x => x.OrderLines).ThenInclude(x => x.Product);


	private IQueryable<Order> ApplyQueryCommand(IQueryable<Order> queryable, QueryCommand command) => 
		QueryCommandApplier.ApplySorting(QueryCommandApplier.ApplyFiltering(queryable, command), command);

}
