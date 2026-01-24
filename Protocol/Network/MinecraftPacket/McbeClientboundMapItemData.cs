using Protocol.Utils;

namespace Protocol.Network.MinecraftPacket;

public class McpeClientboundMapItemData : Packet
{
    public MapInfo mapinfo; 

    public McpeClientboundMapItemData()
    {
        Id = 0x43;
        IsMcpe = true;
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


    protected override void ResetPacket()
    {
        base.ResetPacket();

        mapinfo = default;
    }
}