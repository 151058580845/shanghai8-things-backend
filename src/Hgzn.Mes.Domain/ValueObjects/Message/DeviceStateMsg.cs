using System.ComponentModel;
using System.Text.Json.Nodes;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message.Base;

namespace Hgzn.Mes.Domain.ValueObjects.Message
{
    /// <summary>
    /// 设备状态消息内容
    /// </summary>
    public class DeviceStateMsg : IBinaryMessage
    {
        public DeviceStateMsg(ArraySegment<byte> raw)
        {
            State = (EquipNoticeType)raw[0];
        }

        public EquipNoticeType State { get; set; }
        
        public byte[] FrameHeader { get; set; }
        public byte FrameType { get; set; }
        public byte[] FrameData { get; set; }
        public byte[] AsFrame()
        {
            throw new NotImplementedException();
        }

        public bool Verify(byte[] raw)
        {
            throw new NotImplementedException();
        }
    }
}