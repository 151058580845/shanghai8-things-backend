namespace Hgzn.Mes.Domain.Utilities
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
    public class ScopeDefinitionAttribute : Attribute
    {
        public ScopeDefinitionAttribute(string description, string name)
        {
            Name = name;
            Description = description;            
        }

        public string Name { get; private set; }

        public string Description { get; private set; }
    }
}