using Universe.Core.Exceptions;
using Universe.Core.ModuleBinding.Bindings;
using Universe.Data;
using Microsoft.EntityFrameworkCore;

using System.Reflection;

namespace Universe.Web.ModuleBinding
{
    public class DbCreationModuleBinding : IDbCreationModuleBinding
    {
        public void Create(IServiceScope serviceScope)
        {
            var mainContext = serviceScope.ServiceProvider.GetService<IDataContext>();

            if (mainContext is null)
                throw new StartupRequiredVariableIsEmptyException(Assembly.GetExecutingAssembly().FullName, nameof(IDataContext));

            mainContext.Database.Migrate();
        }
    }
}
