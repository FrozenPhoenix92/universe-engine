using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Universe.Core.AppConfiguration;
using Universe.Core.Infrastructure;
using Universe.Web.Dto;
using Universe.Web.Model;

namespace Universe.Web.Controllers.ControlPanel.Project;

[Route($"{ApiRoutes.ControlPanel}product")]
public class ProductController : DataControllerCore<Product, ProductDto>
{
	public ProductController(IMapper mapper, IDataService<Product> dataService) : base(mapper, dataService)
	{
	}
}
