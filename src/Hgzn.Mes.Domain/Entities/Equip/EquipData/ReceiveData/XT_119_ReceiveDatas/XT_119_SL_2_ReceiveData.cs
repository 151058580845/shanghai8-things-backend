using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_119_ReceiveDatas
{
    /// <summary>
    /// _雷达转台
    /// </summary>
    public class XT_119_SL_2_ReceiveData : UniversalEntity, IAudited
    {
        public Guid? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }

        [Description("仿真试验系统识别编码")]
        public byte SimuTestSysld { get; set; }

        [Description("设备类型识别编码")]
        public byte DevTypeld { get; set; }

        [Description("本机识别编码")]
        public string? Compld { get; set; }

        #region 工作模式信息 3个

        [Description("运行状态")]
        public byte OperationStatus { get; set; }

        [Description("是否接入弹道状态")]
        public byte IsTrajectoryConnected { get; set; }

        [Description("预留")]
        public byte Reserved { get; set; }

        #endregion

        #region 健康状态信息 3个

        [Description("状态类型")]
        public byte StatusType { get; set; }

        [Description("自检状态")]
        public byte SelfTestStatus { get; set; }

        [Description("运行状态")]
        public byte HealthOperationStatus { get; set; }

        #endregion

        #region 物理量

        [Description("物理量参数数量")]
        public uint PhysicalQuantityCount { get; set; }

        [Description("内框位置")]
        public float InnerFramePosition { get; set; }

        [Description("中框位置")]
        public float MiddleFramePosition { get; set; }

        [Description("外框位置")]
        public float OuterFramePosition { get; set; }

        [Description("内框速度")]
        public float InnerFrameVelocity { get; set; }

        [Description("中框速度")]
        public float MiddleFrameVelocity { get; set; }

        [Description("外框速度")]
        public float OuterFrameVelocity { get; set; }

        [Description("内框加速度")]
        public float InnerFrameAcceleration { get; set; }

        [Description("中框加速度")]
        public float MiddleFrameAcceleration { get; set; }

        [Description("外框加速度")]
        public float OuterFrameAcceleration { get; set; }

        #endregion

        [Description("运行时间")]
        public uint? RunTime { get; set; }

        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int CreatorLevel { get; set; } = 0;

#if DEBUG
        public static XT_119_SL_2_ReceiveData[] Seeds
        {
            get
            {
                List<string> uuids = [
                    "0198BC77-48C0-724A-B8AA-0700B2FEA6A0",
                    "0198BC77-48C0-724A-B8AA-0ABD68508542",
                    "0198BC77-48C0-724A-B8AA-0FC4F791B63D",
                    "0198BC77-48C0-724A-B8AA-13C8A4D34B16",
                    "0198BC77-48C0-724A-B8AA-155E4AB869D9",
                    "0198BC77-48C0-724A-B8AA-194CA2451428",
                    "0198BC77-48C0-724A-B8AA-1F620BA0D23B",
                    "0198BC77-48C0-724A-B8AA-201D49063CFF",
                    "0198BC77-48C0-724A-B8AA-247D1679A88E",
                    "0198BC77-48C0-724A-B8AA-2B36C9FFCF30",
                    "0198BC77-48C0-724A-B8AA-2FDC1B473BC1",
                    "0198BC77-48C0-724A-B8AA-3263D88A6B18",
                    "0198BC77-48C0-724A-B8AA-35350CB9B4ED",
                    "0198BC77-48C0-724A-B8AA-39E4E938CD32",
                    "0198BC77-48C0-724A-B8AA-3C19DD165DC8",
                    "0198BC77-48C0-724A-B8AA-41D3B19CB552",
                    "0198BC77-48C0-724A-B8AA-46F43F10D394",
                    "0198BC77-48C0-724A-B8AA-4A7B9EE1046D",
                    "0198BC77-48C0-724A-B8AA-4FDB33B1D7CA",
                    "0198BC77-48C0-724A-B8AA-51D94DCA8695",
                    "0198BC77-48C0-724A-B8AA-550F766E40B5",
                    "0198BC77-48C0-724A-B8AA-590BDDA945DC",
                    "0198BC77-48C0-724A-B8AA-5E950215742C",
                    "0198BC77-48C0-724A-B8AA-610C67066668",
                    "0198BC77-48C0-724A-B8AA-65236D6CA71E",
                    "0198BC77-48C0-724A-B8AA-68759B389C02",
                    "0198BC77-48C0-724A-B8AA-6E9968A63F07",
                    "0198BC77-48C0-724A-B8AA-70EA15B108A8",
                    "0198BC77-48C0-724A-B8AA-75696529CA13",
                    "0198BC77-48C0-724A-B8AA-78AA05EB9B91",
                    "0198BC77-48C0-724A-B8AA-7D8A125923D0",
                    "0198BC77-48C0-724A-B8AA-82A997768A12",
                    "0198BC77-48C0-724A-B8AA-878216D205EF",
                    "0198BC77-48C0-724A-B8AA-8846EE36848B",
                    "0198BC77-48C0-724A-B8AA-8EB0A999FF79",
                    "0198BC77-48C0-724A-B8AA-92D99AC86D8B",
                    "0198BC77-48C0-724A-B8AA-96098043904F",
                    "0198BC77-48C0-724A-B8AA-9B481B7F6A9A",
                    "0198BC77-48C0-724A-B8AA-9D305F34F706",
                    "0198BC77-48C0-724A-B8AA-A1249FC72800",
                    "0198BC77-48C0-724A-B8AA-A7DF3B5FCE46",
                    "0198BC77-48C0-724A-B8AA-ABB0D7D0CEC5",
                    "0198BC77-48C0-724A-B8AA-AF34542C96E7",
                    "0198BC77-48C0-724A-B8AA-B0CE5F4AF0C4",
                    "0198BC77-48C0-724A-B8AA-B78D2638C23D",
                    "0198BC77-48C0-724A-B8AA-B8D55E6D919E",
                    "0198BC77-48C0-724A-B8AA-BEB7936F66E3",
                    "0198BC77-48C0-724A-B8AA-C03A591A068D",
                    "0198BC77-48C0-724A-B8AA-C45DEA185229",
                    "0198BC77-48C0-724A-B8AA-C9EDB25E3735",
                    "0198BC77-48C0-724A-B8AA-CD68E2633DD7",
                    "0198BC77-48C0-724A-B8AA-D13718720E0F",
                    "0198BC77-48C0-724A-B8AA-D62893243C60",
                    "0198BC77-48C0-724A-B8AA-D926FE76F2D1",
                    "0198BC77-48C0-724A-B8AA-DC76A88C5185",
                    "0198BC77-48C0-724A-B8AA-E1FDFF9F0E36",
                    "0198BC77-48C0-724A-B8AA-E4FE2D693197",
                    "0198BC77-48C0-724A-B8AA-E9CF35A5D09B",
                    "0198BC77-48C0-724A-B8AA-EFF7C11E5E6B",
                    "0198BC77-48C0-724A-B8AA-F2CA6F35C4A2",
                    "0198BC77-48C0-724A-B8AA-F75693CEC64B",
                    "0198BC77-48C0-724A-B8AA-FA886810B04B",
                    "0198BC77-48C0-724A-B8AA-FD68F5C87A4F",
                    "0198BC77-48C1-74C9-B905-AB2768D9688D",
                    "0198BC77-48C1-74C9-B905-AE5480AD7B7E",
                    "0198BC77-48C1-74C9-B905-B131984329E4",
                    "0198BC77-48C1-74C9-B905-B429406DF056",
                    "0198BC77-48C1-74C9-B905-BB64747A8B4F",
                    "0198BC77-48C1-74C9-B905-BD19D721BB10",
                    "0198BC77-48C1-74C9-B905-C1246D19CF5E",
                    "0198BC77-48C1-74C9-B905-C73BADBEC9FE",
                    "0198BC77-48C1-74C9-B905-CAAD59A8F25D",
                    "0198BC77-48C1-74C9-B905-CE5421EC5AF9",
                    "0198BC77-48C1-74C9-B905-D09B703EEF02",
                    "0198BC77-48C1-74C9-B905-D6A3D39539B0",
                    "0198BC77-48C1-74C9-B905-DAE48A8893AC",
                    "0198BC77-48C1-74C9-B905-DD72C34E3641",
                    "0198BC77-48C1-74C9-B905-E22CF985B9E0",
                    "0198BC77-48C1-74C9-B905-E6DB7A15E03A",
                    "0198BC77-48C1-74C9-B905-EABA765F4939",
                    "0198BC77-48C1-74C9-B905-EC94CF0F61A0",
                    "0198BC77-48C1-74C9-B905-F2C36FA58F18",
                    "0198BC77-48C1-74C9-B905-F4A4F9929FC5",
                    "0198BC77-48C1-74C9-B905-F8404784C38B",
                    "0198BC77-48C1-74C9-B905-FFF21C01A4EA",
                    "0198BC77-48C1-74C9-B906-009CA3CEFCB9",
                    "0198BC77-48C1-74C9-B906-052ACDFEFC4C",
                    "0198BC77-48C1-74C9-B906-08148CD118EC",
                    "0198BC77-48C1-74C9-B906-0FB626F7E6DB",
                    "0198BC77-48C1-74C9-B906-134FF5D3F54B",
                    "0198BC77-48C1-74C9-B906-17DD2AEF52CC",
                    "0198BC77-48C1-74C9-B906-1ABE4644E6F7",
                    "0198BC77-48C1-74C9-B906-1E57033A3707",
                    "0198BC77-48C1-74C9-B906-21141593F134",
                    "0198BC77-48C1-74C9-B906-266A977260DE",
                    "0198BC77-48C1-74C9-B906-2BC61ED158E8",
                    "0198BC77-48C1-74C9-B906-2EBEF1D23F24",
                    "0198BC77-48C1-74C9-B906-33822C66D069",
                    "0198BC77-48C1-74C9-B906-35EEBA7D1B84",
                    "0198BC77-48C1-74C9-B906-39C2D9E3CE3C"
                ];
                List<XT_119_SL_2_ReceiveData> list = [];
                List<string> equipUuids = [
                    "0198BBB3A649726DBD9F60682462B0B0",
                    "0198BBB3A649726DBD9F648070A10222",
                    "0198BBB3A649726DBD9F687CDF069C0E",
                    "0198BBB3A649726DBD9F6E1DE0BABBB1",
                    "0198BBB3A649726DBD9F719218D7D417",
                    "0198BBB3A649726DBD9F75673C37938F",
                    "0198BBB3A649726DBD9F7B0B2A68A69F",
                    "0198BBB3A649726DBD9F7E7AD72E8FC2",
                    "0198BBB3A649726DBD9F807E21B45F26",
                    "0198BBB3A649726DBD9F87312AFA35E0"
                ];
                var rand = new Random();
                var now = DateTime.Now;
                int a = 0;
                foreach (var uuid in uuids)
                {
                    var innerFrameVelocity = rand.NextDouble() * 40 + 20;
                    var innerFrameAcceleration = rand.NextDouble() * 19.9 + 0.1;
                    list.Add(new XT_119_SL_2_ReceiveData
                    {
                        Id = Guid.Parse(uuid),
                        SimuTestSysld = 10,
                        DevTypeld = 2,
                        Compld = equipUuids[rand.Next(equipUuids.Count - 1)],
                        OperationStatus = 1,
                        IsTrajectoryConnected = 1,
                        Reserved = 1,
                        StatusType = 1,
                        SelfTestStatus = 1,
                        HealthOperationStatus = 1,
                        PhysicalQuantityCount = 9,
                        InnerFramePosition = rand.Next(-180, 180),
                        MiddleFramePosition = rand.Next(-120, 120),
                        OuterFramePosition = rand.Next(-180, 180),
                        InnerFrameVelocity = (float)innerFrameVelocity,
                        MiddleFrameVelocity = (float)(rand.NextDouble() * ((innerFrameVelocity < 30 ? innerFrameVelocity : 30) - 0.1) + 0.1),
                        OuterFrameVelocity = (float)(rand.NextDouble() * 19.9 + 0.1),
                        InnerFrameAcceleration = (float)innerFrameAcceleration,
                        MiddleFrameAcceleration = (float)(rand.NextDouble() * ((innerFrameAcceleration < 10 ? innerFrameVelocity : 10) - 0.05) + 0.05),
                        OuterFrameAcceleration = (float)(rand.NextDouble() * 4.95 + 0.05),
                        RunTime = (uint?)new DateTimeOffset(now.AddSeconds(a)).ToUnixTimeSeconds(),
                        CreationTime = now.AddSeconds(a)
                    });
                    a++;
                }
                return [.. list];
            }
        }
#endif
    }
}
