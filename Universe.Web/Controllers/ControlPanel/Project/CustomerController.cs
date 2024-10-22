using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Universe.Core.AppConfiguration;
using Universe.Core.Infrastructure;
using Universe.Web.Dto;
using Universe.Web.Model;

namespace Universe.Web.Controllers.ControlPanel.Project;

[Route($"{ApiRoutes.ControlPanel}customer")]
public class CustomerController : DataControllerCore<Customer, CustomerDto>
{
	public CustomerController(IMapper mapper, IDataService<Customer> dataService) : base(mapper, dataService) 
	{
	}
}
