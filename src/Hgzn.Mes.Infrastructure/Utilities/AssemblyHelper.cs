using System.Reflection;

namespace Hgzn.Mes.Infrastructure.Helpers;

public class AssemblyHelper
{
    public static Assembly[] GetAssembliesApplication()
    {
        return GetAssembliesByName("Hgzn.Mes." + nameof(Hgzn.Mes.Application)).ToArray();
    }
    public static List<Assembly> GetAssembliesByName(string name)
    {
      return  AppDomain.CurrentDomain.GetAssemblies()
          .Where(t => t.FullName != null && t.FullName.Contains(name))
          .ToList();
    }
}