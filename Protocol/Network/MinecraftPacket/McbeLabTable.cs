namespace Protocol.Network.MinecraftPacket;

public class McpeLabTable : Packet
{
    public int labTableX; 
    public int labTableY; 
    public int labTableZ; 
    public byte reactionType; 

    public byte uselessByte; 

    public McpeLabTable()
    {
        Id = 0x6d;
        IsMcpe = true;
    }

    protected override void EncodePacket()
    {
        base.EncodePacket();


        Write(uselessByte);
        WriteVarInt(labTableX);
        WriteVarInt(labTableY);
        WriteVarInt(labTableZ);
        Write(reactionType);
    }


    protected override void DecodePacket()
    {
        base.DecodePacket();


        uselessByte = ReadByte();
        labTableX = ReadVarInt();
        labTableY = ReadVarInt();
        labTableZ = ReadVarInt();
        reactionType = ReadByte();
    }


    protected override void ResetPacket()
    {
        base.ResetPacket();

        uselessByte = default;
        labTableX = default;
        labTableY = default;
        labTableZ = default;
        reactionType = default;
    }
}