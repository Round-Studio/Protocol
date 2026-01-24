

namespace Protocol.Network.MinecraftPacket;




public class McpeAwardAchievement : Packet
{
    
    
    
    public McpeAwardAchievement()
    {
        Id = 309; 
        IsMcpe = true;
    }

    
    
    
    public int AchievementID { get; set; } 

    
    
    
    protected override void EncodePacket()
    {
        base.EncodePacket();

        
        
        
        Write(AchievementID); 
    }

    
    
    
    protected override void DecodePacket()
    {
        base.DecodePacket();

        
        
        
        AchievementID = ReadInt(); 
    }

    
    
    
    protected override void ResetPacket()
    {
        base.ResetPacket();
        AchievementID = 0;
    }
}