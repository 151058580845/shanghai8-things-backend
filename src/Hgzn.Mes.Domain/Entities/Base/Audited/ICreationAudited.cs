namespace Hgzn.Mes.Domain.Entities.System.Base.Audited
{
    public interface ICreationAudited
    {
        Guid? CreatorId { get; set; }
        DateTime CreationTime { get; set; }
    }
}
