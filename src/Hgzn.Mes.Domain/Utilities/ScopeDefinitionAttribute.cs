using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Domain.Utilities
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
    public class ScopeDefinitionAttribute : Attribute
    {
        public ScopeDefinitionAttribute(ScopeMethodType type, string? extend = null, string? description = null)
        {
            _type = type;
            _extend = extend;
            Description = description;
        }

        public string? _extend;

        public string? Description { get; private set; }

        private ScopeMethodType _type;

        public override string ToString() {
            return _type switch
            {
                ScopeMethodType.Query or ScopeMethodType.List or ScopeMethodType.Remove or
                ScopeMethodType.Add or ScopeMethodType.Edit => _type.ToString().ToLower(),
                ScopeMethodType.Extend => _extend ?? throw new ArgumentNullException(nameof(_extend)),
                _ => throw new ArgumentOutOfRangeException(nameof(_type))
            };
        }
    }
}