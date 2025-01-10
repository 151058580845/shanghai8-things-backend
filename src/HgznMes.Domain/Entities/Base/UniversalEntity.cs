using System.ComponentModel.DataAnnotations;

namespace HgznMes.Domain.Entities.System.Base
{
    public abstract class UniversalEntity : IAggregateRoot
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}