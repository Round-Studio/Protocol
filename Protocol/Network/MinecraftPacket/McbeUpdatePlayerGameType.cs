namespace Protocol.Network.MinecraftPacket;
public class McbeUpdatePlayerGameType : Packet
{
    public McbeUpdatePlayerGameType()
    {
        Id = 0x97;
        IsMcbe = true;
    }

    protected override void EncodePacket()
    {
        base.EncodePacket();
    }

    protected override void DecodePacket()
    {
        base.DecodePacket();
    }
}