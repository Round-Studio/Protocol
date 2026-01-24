

namespace Protocol.Network.MinecraftPacket;





public class McpeScriptMessage : Packet
{
    
    
    
    public McpeScriptMessage()
    {
        Id = 177; 
        IsMcpe = true;
    }

    
    
    
    public string Identifier { get; set; } = string.Empty; 

    
    
    
    public byte[] Data { get; set; } = new byte[0]; 

    
    
    
    protected override void EncodePacket()
    {
        base.EncodePacket();

        
        Write(Identifier);

        
        
        WriteByteArray(Data);
    }

    
    
    
    protected override void DecodePacket()
    {
        base.DecodePacket();

        
        Identifier = ReadString();

        
        
        
        
        
        Data = ReadByteArray(true); 
    }

    
    
    
    protected override void ResetPacket()
    {
        base.ResetPacket();
        Identifier = string.Empty;
        Data = new byte[0];
    }
}