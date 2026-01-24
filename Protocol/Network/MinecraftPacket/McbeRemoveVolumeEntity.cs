

namespace Protocol.Network.MinecraftPacket;




public class McpeRemoveVolumeEntity : Packet
{
    
    
    
    public McpeRemoveVolumeEntity()
    {
        Id = 167; 
        IsMcpe = true;
    }

    
    
    
    public ulong EntityRuntimeID { get; set; } 

    
    
    
    public int Dimension { get; set; } 

    
    
    
    protected override void EncodePacket()
    {
        base.EncodePacket();

        
        Write(EntityRuntimeID);

        
        
        WriteSignedVarInt(Dimension);
    }

    
    
    
    protected override void DecodePacket()
    {
        base.DecodePacket();

        
        EntityRuntimeID = ReadUlong();

        
        Dimension = ReadSignedVarInt();
    }

    
    
    
    protected override void ResetPacket()
    {
        base.ResetPacket();
        EntityRuntimeID = 0;
        Dimension = 0;
    }
}