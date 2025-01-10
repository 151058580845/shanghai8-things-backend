using System.ComponentModel.DataAnnotations;

namespace Hgzn.Mes.Domain.Entities.Base
{
    public abstract class UniversalEntity : AggregateRoot
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}