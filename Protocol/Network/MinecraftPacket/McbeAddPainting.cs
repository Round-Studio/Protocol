using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McpeAddPainting : Packet
{
    public BlockCoordinates coordinates; 
    public int direction; 

    public long entityIdSelf; 
    public long runtimeEntityId; 
    public string title; 

    public McpeAddPainting()
    {
        Id = 0x16;
        IsMcpe = true;
    }

    protected override void EncodePacket()
    {
        base.EncodePacket();


        WriteSignedVarLong(entityIdSelf);
        WriteUnsignedVarLong(runtimeEntityId);
        WritePaintingCoordinates(coordinates);
        WriteSignedVarInt(direction);
        Write(title);
    }


    protected override void DecodePacket()
    {
        base.DecodePacket();


        entityIdSelf = ReadSignedVarLong();
        runtimeEntityId = ReadUnsignedVarLong();
        coordinates = ReadBlockCoordinates();
        direction = ReadSignedVarInt();
        title = ReadString();
    }


    protected override void ResetPacket()
    {
        base.ResetPacket();

        entityIdSelf = default;
        runtimeEntityId = default;
        coordinates = default;
        direction = default;
        title = default;
    }
}