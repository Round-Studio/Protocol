using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McpeSyncEntityProperty : Packet
{
    public Nbt propertyData; 

    public McpeSyncEntityProperty()
    {
        Id = 0xa5;
        IsMcpe = true;
    }

    protected override void EncodePacket()
    {
        base.EncodePacket();


        Write(propertyData);
    }


    protected override void DecodePacket()
    {
        base.DecodePacket();


        propertyData = ReadNbt();
    }


    protected override void ResetPacket()
    {
        base.ResetPacket();

        propertyData = default;
    }
}