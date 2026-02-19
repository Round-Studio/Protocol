using Protocol.Minecraft.Map;

namespace Protocol.Network.MinecraftPacket;
public class McbeClientboundMapItemData : Packet
{
    public MapInfo mapinfo;
    public McbeClientboundMapItemData()
    {
        Id = 0x43;
        IsMcbe = true;
    }

    protected override void EncodePacket()
    {
        base.EncodePacket();
        Write(mapinfo);
    }

    protected override void DecodePacket()
    {
        base.DecodePacket();
        mapinfo = ReadMapInfo();
    }
}