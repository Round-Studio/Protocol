

namespace Protocol.Network.MinecraftPacket;





public class McpeGameTestResults : Packet
{
    
    
    
    public McpeGameTestResults()
    {
        Id = 195; 
        IsMcpe = true;
    }

    
    
    
    public string Name { get; set; } = string.Empty; 

    
    
    
    public bool Succeeded { get; set; } 

    
    
    
    public string Error { get; set; } = string.Empty; 

    
    
    
    protected override void EncodePacket()
    {
        base.EncodePacket();

        
        Write(Succeeded);

        
        Write(Error);

        
        Write(Name);
    }

    
    
    
    protected override void DecodePacket()
    {
        base.DecodePacket();

        
        Succeeded = ReadBool();

        
        Error = ReadString();

        
        Name = ReadString();
    }

    
    
    
    protected override void ResetPacket()
    {
        base.ResetPacket();
        Name = string.Empty;
        Succeeded = false;
        Error = string.Empty;
    }
}