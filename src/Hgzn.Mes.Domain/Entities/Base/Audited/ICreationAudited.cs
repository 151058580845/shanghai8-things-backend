﻿namespace Hgzn.Mes.Domain.Entities.Base.Audited
{
    public interface ICreationAudited
    {
        Guid? CreatorId { get; set; }
        DateTime CreationTime { get; set; }
        int CreatorLevel { get; set; }
    }
}