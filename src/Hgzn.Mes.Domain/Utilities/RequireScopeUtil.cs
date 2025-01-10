using Hgzn.Mes.Domain.ValueObjects;
using System.Reflection;

namespace Hgzn.Mes.Domain.Utilities
{
    public static class RequireScopeUtil
    {
        public static ScopeDefinition[] Scopes { get; set; } = null!;

        public static void Initialize()
        {
            Scopes = Assembly.Load("Hgzn.Mes." + nameof(Application)+".Main").GetTypes()
                .Where(type => type.Namespace == "Hgzn.Mes." + nameof(Application) + ".Services")
                .SelectMany(rqt => rqt.GetMethods()
                    .Select(m => (typeName: rqt.Name, attribute: m.GetCustomAttribute<ScopeDefinitionAttribute>()))
                    .Append((typeName: rqt.Name, attribute: rqt.GetCustomAttribute<ScopeDefinitionAttribute>())))
                .Where(t => t.attribute is not null)
                .Select(tuple => new ScopeDefinition
                {
                    Name = tuple.attribute!.Name,
                    Description = tuple.attribute!.Description
                })
                .ToArray();
        }

        public static bool IsExist(string scopeName) => Scopes.Any(s => s.Name == scopeName);

        public static ScopeDefinition Fill(string scopeName) => Scopes.First(s => s.Name == scopeName);

        public static IEnumerable<ScopeDefinition> FillAll(IEnumerable<string> scopeNames)
        {
            var result = new List<ScopeDefinition>();
            var enumerator = scopeNames.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var fullScope = Fill(enumerator.Current);
                if (fullScope is null)
                {
                    continue;
                }
                else
                {
                    result.Add(fullScope);
                }
            }
            return result;
        }

        public static bool TryFillAll(IEnumerable<string> scopeNames, out List<ScopeDefinition> fullScopes)
        {
            var result = new List<ScopeDefinition>();
            var enumerator = scopeNames.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var fullScope = Fill(enumerator.Current);
                if (fullScope is null)
                {
                    fullScopes = result;
                    return false;
                }
                else
                {
                    result.Add(fullScope);
                }
            }
            fullScopes = result;
            return true;
        }
    }
}