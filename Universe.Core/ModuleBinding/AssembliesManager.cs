using System.Reflection;

namespace Universe.Core.ModuleBinding;

public static class AssembliesManager
{
    public const string ProjectAssemblyPrefix = "Universe.";


    static AssembliesManager() => PreloadAssemblies();


    public static IEnumerable<Assembly> GetProjectAssemblies() =>
        AppDomain.CurrentDomain.GetAssemblies().Where(o => o.FullName?.StartsWith(ProjectAssemblyPrefix) is true);

    public static List<T> GetInstances<T>()
    {
        var linkerType = typeof(T);

        var linkers = new List<T>();
        foreach (var assembly in GetProjectAssemblies())
        {
            var linkerClasses = assembly.GetTypes().Where(p =>
                p.IsClass && !p.IsAbstract && !p.ContainsGenericParameters && linkerType.IsAssignableFrom(p));

            foreach (var linkerClass in linkerClasses)
            {
                if (!string.IsNullOrEmpty(linkerClass?.FullName))
                {
                    var linkerInstance = (T?) assembly.CreateInstance(linkerClass.FullName);

                    if (linkerInstance != null)
                    {
                        linkers.Add(linkerInstance);
                    }
                }
            }
        }

        return linkers;
    }

    public static bool FromProjectAssembly(Type type) => type.Assembly?.FullName?.StartsWith(ProjectAssemblyPrefix) is true;


    private static void PreloadAssemblies()
    {
        foreach (var assembly in GetProjectAssemblies())
            LoadReferencedAssembly(assembly);
    }

    private static void LoadReferencedAssembly(Assembly assembly)
    {
        var refAssembliesNames = assembly.GetReferencedAssemblies().Where(o => o.FullName.StartsWith(ProjectAssemblyPrefix));

        foreach (AssemblyName name in refAssembliesNames)
        {
            var childAssembly = GetProjectAssemblies().FirstOrDefault(a => a.FullName == name.FullName);
            if (childAssembly is null)
                childAssembly = Assembly.Load(name);

            LoadReferencedAssembly(childAssembly);
        }
    }
}
