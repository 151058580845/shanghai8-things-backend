using System.ComponentModel.DataAnnotations;

namespace Hgzn.Mes.Domain.Entities.System.Base
{
    public class UniversalEntity : IAggregateRoot
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}