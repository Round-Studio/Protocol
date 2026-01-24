using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McpeMobArmorEquipment : Packet
{
    public Item body; 
    public Item boots; 
    public Item chestplate; 
    public Item helmet; 
    public Item leggings; 

    public long runtimeEntityId; 

    public McpeMobArmorEquipment()
    {
        Id = 0x20;
        IsMcpe = true;
    }

    protected override void EncodePacket()
    {
        base.EncodePacket();


        WriteUnsignedVarLong(runtimeEntityId);
        Write(helmet);
        Write(chestplate);
        Write(leggings);
        Write(boots);
        Write(body);
    }


    protected override void DecodePacket()
    {
        base.DecodePacket();


        runtimeEntityId = ReadUnsignedVarLong();
        helmet = ReadItem();
        chestplate = ReadItem();
        leggings = ReadItem();
        boots = ReadItem();
        body = ReadItem();
    }


    protected override void ResetPacket()
    {
        base.ResetPacket();

        runtimeEntityId = default;
        helmet = default;
        chestplate = default;
        leggings = default;
        boots = default;
        body = default;
    }
}