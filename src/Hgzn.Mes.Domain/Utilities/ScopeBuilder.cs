namespace Hgzn.Mes.Application.Auth
{
    public class ScopeBuilder
    {
        private string[] _scopes = new string[3];

        public ScopeBuilder WithAction(string action)
        {
            _scopes[1] = action;
            return this;
        }

        public ScopeBuilder WithResource(string resource)
        {
            _scopes[0] = resource;
            return this;
        }

        public ScopeBuilder WithSuffix(string suffix)
        {
            _scopes[2] = suffix;
            return this;
        }

        public string Build()
        {
            return string.Join('.', _scopes);
        }
    }
}