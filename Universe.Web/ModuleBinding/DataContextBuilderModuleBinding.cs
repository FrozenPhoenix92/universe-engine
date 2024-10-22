using Universe.Core;
using Universe.Core.ModuleBinding.Bindings;
using Universe.Web.Model;

using Microsoft.EntityFrameworkCore;

namespace Universe.Web.ModuleBinding;

public class DataContextBuilderModuleBinding : IDataContextBuilderModuleBinding
{
	public string ModuleBindingId => CoreStaticData.MainDataContextBindingId;

	public void ConfigureModels(ModelBuilder builder)
	{
		builder.Entity<Client>();
		builder.Entity<ClientOperationConfirmationToken>();
		builder.Entity<Customer>();
		builder.Entity<Product>();
		builder.Entity<Order>();
		builder.Entity<OrderLine>();
	}
}
