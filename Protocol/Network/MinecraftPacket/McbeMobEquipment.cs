using Protocol.Minecraft;


namespace Protocol.Network.MinecraftPacket;

public class McpeMobEquipment : Packet
{
    public Item item; 

    public long runtimeEntityId; 
    public byte selectedSlot; 
    public byte slot; 
    public byte windowsId; 

    public McpeMobEquipment()
    {
        Id = 0x1f;
        IsMcpe = true;
    }

    protected override void EncodePacket()
    {
        base.EncodePacket();


        WriteUnsignedVarLong(runtimeEntityId);
        Write(item);
        Write(slot);
        Write(selectedSlot);
        Write(windowsId);
    }


    protected override void DecodePacket()
    {
        base.DecodePacket();


        runtimeEntityId = ReadUnsignedVarLong();
        item = ReadItem();
        slot = ReadByte();
        selectedSlot = ReadByte();
        windowsId = ReadByte();
    }


    protected override void ResetPacket()
    {
        base.ResetPacket();

        runtimeEntityId = default;
        item = default;
        slot = default;
        selectedSlot = default;
        windowsId = default;
    }
}