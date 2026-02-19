namespace Protocol.Network.MinecraftPacket;
public class McbePermissionRequest : Packet
{
    public short flagss;
    public uint permission;
    public long runtimeEntityId;
    public McbePermissionRequest()
    {
        Id = 0xb9;
        IsMcbe = true;
    }

    protected override void EncodePacket()
    {
        base.EncodePacket();
    }

    protected override void DecodePacket()
    {
        base.DecodePacket();
        runtimeEntityId = ReadLong();
        permission = ReadUnsignedVarInt();
        flagss = ReadShort();
    }
}