using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Universe.Core.AppConfiguration;
using Universe.Core.Infrastructure;
using Universe.Core.QueryHandling;
using Universe.Web.Dto;
using Universe.Web.Model;
using Universe.Web.Services;

namespace Universe.Web.Controllers.ControlPanel.Project;

[Route($"{ApiRoutes.ControlPanel}order")]
public class OrderController : DataControllerCore<Order, OrderDto>
{
	private readonly IOrderService _orderService;


	public OrderController(IMapper mapper, IDataService<Order> dataService, IOrderService orderService)
		: base(mapper, dataService, orderService) => _orderService = orderService;


	[HttpGet("orders-sum")]
	public async Task<IActionResult> GetOrdersSum([FromQuery] QueryCommand command, CancellationToken ct = default)
		=> Ok(await _orderService.GetOrdersSum(command, ct));

	[HttpGet("most-frequent-product")]
	public async Task<IActionResult> GetMostFrequentProduct([FromQuery] QueryCommand command, CancellationToken ct = default)
		=> Ok(await _orderService.GetMostFrequentProduct(command, ct));
}
