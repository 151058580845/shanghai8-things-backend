
namespace Hgzn.Mes.Domain.Shared.Extensions
{
    public static class TypeExtension
    {
        public static bool IsDatabaseType(this Type type)
        {
            return (!type.IsClass || type == typeof(string)) && !type.IsInterface;
        }
    }
}
