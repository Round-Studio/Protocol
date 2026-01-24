namespace Protocol.Network.MinecraftPacket;

public class McpeAnimate : Packet
{
    public int actionId; 
    public long runtimeEntityId; 
    public float Data;
    public float unknownFloat;

    public McpeAnimate()
    {
        Id = 0x2c;
        IsMcpe = true;
    }

    protected override void EncodePacket()
    {
        base.EncodePacket();


        WriteSignedVarInt(actionId);
        WriteUnsignedVarLong(runtimeEntityId);
        Write(Data);
        if (actionId == 0x80 || actionId == 0x81) Write(unknownFloat);
    }


    protected override void DecodePacket()
    {
        base.DecodePacket();


        actionId = ReadSignedVarInt();
        runtimeEntityId = ReadUnsignedVarLong();
        Data =  ReadFloat();
		if (actionId == 0x80 || actionId == 0x81) unknownFloat = ReadFloat();
    }


    protected override void ResetPacket()
    {
        base.ResetPacket();

        actionId = default;
        runtimeEntityId = default;
        unknownFloat = default;
        Data = default;
    }
}