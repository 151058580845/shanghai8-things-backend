namespace Hgzn.Mes.Domain.ValueObjects
{
    public class ScopeDefinition
    {
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
    }

    public static class ScopeMethodType
    {
        public const string Query = "query";
        public const string List = "list";
        public const string Remove = "remove";
        public const string Add = "add";
        public const string Edit = "edit";
    }
}