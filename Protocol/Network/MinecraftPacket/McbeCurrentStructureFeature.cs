

namespace Protocol.Network.MinecraftPacket;




public class McpeCurrentStructureFeature : Packet
{
    
    
    
    public McpeCurrentStructureFeature()
    {
        Id = 314; 
        IsMcpe = true;
    }

    
    
    
    
    public string CurrentFeature { get; set; } = string.Empty; 

    
    
    
    protected override void EncodePacket()
    {
        base.EncodePacket();

        
        Write(CurrentFeature);
    }

    
    
    
    protected override void DecodePacket()
    {
        base.DecodePacket();

        
        CurrentFeature = ReadString();
    }

    
    
    
    protected override void ResetPacket()
    {
        base.ResetPacket();
        CurrentFeature = string.Empty;
    }
}