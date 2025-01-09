namespace HgznMes.Domain.Entities.System.Base.Audited
{
    public interface ILastModificationAudited
    {
        Guid? LastModifierId { get; set; }
        DateTime? LastModificationTime { get; set; }
    }
}
