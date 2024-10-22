using AutoMapper;

using Universe.Core.AppConfiguration;
using Universe.Core.Common.Dto;
using Universe.Core.Common.Model;
using Universe.Core.Infrastructure;

using Microsoft.AspNetCore.Mvc;

namespace Universe.Web.Controllers.ControlPanel.Common;

[Route($"{ApiRoutes.ControlPanel}ip-address-constraint")]
public class IpAddressConstraintController : DataControllerCore<IpAddressConstraint, IpAddressConstraintDto>
{
	public IpAddressConstraintController(IMapper mapper, IDataService<IpAddressConstraint> defaultDataOperationsService)
		: base(mapper, defaultDataOperationsService)
	{
	}
}
