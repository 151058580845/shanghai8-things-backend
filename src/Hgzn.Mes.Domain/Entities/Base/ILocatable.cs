
using Hgzn.Mes.Domain.Entities.System.Location;

namespace Hgzn.Mes.Domain.Entities.Base
{
    public interface ILocatable
    {
        public bool IsMoving { get; set; }

        public DateTime? LastMoveTime { get; set; }

        public string? PosTags { get; set; }

        public Guid? RoomId { get; set; }

        public Room? Room { get; set; }
    }
}
