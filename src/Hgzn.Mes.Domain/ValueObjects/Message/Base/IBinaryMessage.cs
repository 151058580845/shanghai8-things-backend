namespace Hgzn.Mes.Domain.ValueObjects.Message.Base
{
    public interface IBinaryMessage : IIotMessage
    {
        static byte[] DefaultFrameHeader { get => new byte[] { 0xA5, 0x5A }; }
        byte[] FrameHeader { get; set; }
        int FrameLength { get => 5 + FrameData.Length; }
        byte FrameType { get; set; }
        byte[] FrameData { get; set; }

        byte[] AsFrame();

        bool Verify(byte[] raw);
    }
}