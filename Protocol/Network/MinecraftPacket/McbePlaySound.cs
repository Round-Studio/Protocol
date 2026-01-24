using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McpePlaySound : Packet
{
    public BlockCoordinates coordinates; 

    public string name; 
    public float pitch; 
    public float volume; 

    public McpePlaySound()
    {
        Id = 0x56;
        IsMcpe = true;
    }

    protected override void EncodePacket()
    {
        base.EncodePacket();


        Write(name);
        Write(coordinates);
        Write(volume);
        Write(pitch);
    }


    protected override void DecodePacket()
    {
        base.DecodePacket();


        name = ReadString();
        coordinates = ReadBlockCoordinates();
        volume = ReadFloat();
        pitch = ReadFloat();
    }


    protected override void ResetPacket()
    {
        base.ResetPacket();

        name = default;
        coordinates = default;
        volume = default;
        pitch = default;
    }
}