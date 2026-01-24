namespace Protocol.Network.MinecraftPacket;

public class McpeWrapper : Packet
{
    public ReadOnlyMemory<byte> payload; 

    public McpeWrapper()
    {
        Id = 0xfe;
        IsMcpe = false;
    }

    protected override void EncodePacket()
    {
        base.EncodePacket();

        Write(payload);
    }


    protected override void DecodePacket()
    {
        base.DecodePacket();

        payload = ReadReadOnlyMemory(0, true);
    }


    protected override void ResetPacket()
    {
        base.ResetPacket();
        payload = default;
    }
}