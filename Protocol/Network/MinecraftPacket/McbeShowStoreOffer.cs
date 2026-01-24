using Protocol.Utils;

namespace Protocol.Network.MinecraftPacket;

public class McpeShowStoreOffer : Packet
{
    public UUID OfferID; 
    public byte type; 

    public McpeShowStoreOffer()
    {
        Id = 0x5b;
        IsMcpe = true;
    }

    protected override void EncodePacket()
    {
        base.EncodePacket();


        Write(OfferID);
        Write(type);
    }


    protected override void DecodePacket()
    {
        base.DecodePacket();


        OfferID = ReadUUID();
        type = ReadByte();
    }


    protected override void ResetPacket()
    {
        base.ResetPacket();

        OfferID = default;
        type = default;
    }
}