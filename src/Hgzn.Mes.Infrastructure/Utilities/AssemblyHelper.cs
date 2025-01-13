using System.Reflection;

namespace Hgzn.Mes.Infrastructure.Utilities;

public class AssemblyHelper
{
    public static Assembly[] GetAssembliesApplication()
    {
        return GetAssembliesByName("Hgzn.Mes." + nameof(Application)).ToArray();
    }
    public static List<Assembly> GetAssembliesByName(string name)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(t => t.FullName != null && t.FullName.Contains(name))
            .ToList();
    }
}