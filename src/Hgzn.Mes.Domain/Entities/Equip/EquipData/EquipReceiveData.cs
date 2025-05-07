using Hgzn.Mes.Domain.Entities.Base;
using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_307_ReceiveDatas;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData
{
    public class EquipReceiveData : UniversalEntity,ICreationAudited
    {
        [Description("设备主键")] 
        public Guid? EquipId { get; set; }

        [Description("连接编号")] 
        public Guid? ConnectId { get; set; }
        
        [Description("数据主键")] 
        public Guid? ReceiveDataId { get; set; }
        
        [Description("设备编号")] 
        public string? EquipCode { get; set; }
        
        public XT_307_SL_1_ReceiveData? ReceiveData { get; set; }
        public Guid? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }
    }
}