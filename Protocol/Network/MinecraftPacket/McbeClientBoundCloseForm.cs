

namespace Protocol.Network.MinecraftPacket;





public class McpeClientBoundCloseForm : Packet
{
    
    
    
    public McpeClientBoundCloseForm()
    {
        Id = 310; 
        IsMcpe = true;
    }

    
    
    
    protected override void EncodePacket()
    {
        base.EncodePacket();
        
        
    }

    
    
    
    protected override void DecodePacket()
    {
        base.DecodePacket();
        
        
    }

    
    
    
    protected override void ResetPacket()
    {
        base.ResetPacket();
        
    }
}