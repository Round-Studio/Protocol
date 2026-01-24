namespace Protocol.Network.MinecraftPacket;

public class McpeCodeBuilder : Packet
{
    
    
    
    public McpeCodeBuilder()
    {
        Id = 149; 
        IsMcpe = true; 
    }

    
    
    
    
    public string URL { get; set; } = "";

    
    
    
    
    public bool ShouldOpenCodeBuilder { get; set; }

    
    
    
    protected override void EncodePacket()
    {
        base.EncodePacket(); 

        Write(URL);
        Write(ShouldOpenCodeBuilder);
    }

    
    
    
    protected override void DecodePacket()
    {
        base.DecodePacket(); 

        URL = ReadString();
        ShouldOpenCodeBuilder = ReadBool();
    }
}