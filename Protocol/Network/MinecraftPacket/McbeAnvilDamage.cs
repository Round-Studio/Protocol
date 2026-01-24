using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McpeAnvilDamage : Packet
{
    public BlockCoordinates coordinates; 

    public byte damageAmount; 

    public McpeAnvilDamage()
    {
        Id = 0x8D;
        IsMcpe = true;
    }

    protected override void EncodePacket()
    {
        base.EncodePacket();


        Write(damageAmount);
        Write(coordinates);
    }


    protected override void DecodePacket()
    {
        base.DecodePacket();


        damageAmount = ReadByte();
        coordinates = ReadBlockCoordinates();
    }


    protected override void ResetPacket()
    {
        base.ResetPacket();

        damageAmount = default;
        coordinates = default;
    }
}