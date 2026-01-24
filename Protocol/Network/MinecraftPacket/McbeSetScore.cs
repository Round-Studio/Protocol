using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McpeSetScore : Packet
{
    public enum ChangeTypes
    {
        Player = 1,
        Entity = 2,
        FakePlayer = 3
    }

    public enum Types
    {
        Change = 0,
        Remove = 1
    }

    public ScoreEntries entries; 

    public McpeSetScore()
    {
        Id = 0x6c;
        IsMcpe = true;
    }

    protected override void EncodePacket()
    {
        base.EncodePacket();


        Write(entries);
    }


    protected override void DecodePacket()
    {
        base.DecodePacket();


        entries = ReadScoreEntries();
    }


    protected override void ResetPacket()
    {
        base.ResetPacket();

        entries = default;
    }
}