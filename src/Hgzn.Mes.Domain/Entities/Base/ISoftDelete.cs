namespace Hgzn.Mes.Domain.Entities.System.Base
{
    public interface ISoftDelete
    {
        public bool SoftDeleted { get; set; }

        public DateTime? DeleteTime { get; set; }
    }
}