using Universe.Core;
using Universe.Core.ModuleBinding.Bindings;
using Universe.Messaging.Dto;
using Universe.Messaging.Model;

using Microsoft.EntityFrameworkCore;

namespace Universe.Messaging.ModuleBindings;

public class DataContextBuilderModuleBinding : IDataContextBuilderModuleBinding
{
	public string ModuleBindingId => CoreStaticData.MainDataContextBindingId;

	public void ConfigureModels(ModelBuilder builder)
	{
		builder.Entity<EmailTemplate>();
		builder.Entity<TemplateVariable>();
		builder.Entity<EmailTemplateTemplateVariable>();
	}
}
