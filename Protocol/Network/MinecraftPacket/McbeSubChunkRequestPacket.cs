using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;
public class McbeSubChunkRequestPacket : Packet
{
    public BlockCoordinates basePosition;
    public int dimension;
    public SubChunkPositionOffset[] offsets;
    public McbeSubChunkRequestPacket()
    {
        Id = 0xaf;
        IsMcbe = true;
    }

    protected override void EncodePacket()
    {
        base.EncodePacket();
        WriteVarInt(dimension);
        Write(basePosition);
        Write(offsets);
    }

    protected override void DecodePacket()
    {
        base.DecodePacket();
        dimension = ReadVarInt();
        basePosition = ReadBlockCoordinates();
        offsets = ReadSubChunkPositionOffsets();
    }
}