using Hgzn.Mes.Application.Main.Dtos.Base;

namespace Hgzn.Mes.Application.Main.Dtos.Equip;

public class TestEquipDataQueryDto : PaginatedQueryDto
{
    public int SystemId { get; set; }
    public int EquipTypeId { get; set; }
    public List<string>? AssetNumbers { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
}

public class TestEquipDataReadDto : ReadDto
{
    
}
