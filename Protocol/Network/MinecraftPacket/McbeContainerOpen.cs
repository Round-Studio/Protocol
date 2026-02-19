using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;
public class McbeContainerOpen : Packet
{
    public BlockCoordinates coordinates;
    public long runtimeEntityId;
    public byte type;
    public byte windowId;
    public McbeContainerOpen()
    {
        Id = 0x2e;
        IsMcbe = true;
    }

    protected override void EncodePacket()
    {
        base.EncodePacket();
        Write(windowId);
        Write(type);
        Write(coordinates);
        WriteSignedVarLong(runtimeEntityId);
    }

    protected override void DecodePacket()
    {
        base.DecodePacket();
        windowId = ReadByte();
        type = ReadByte();
        coordinates = ReadBlockCoordinates();
        runtimeEntityId = ReadSignedVarLong();
    }
}