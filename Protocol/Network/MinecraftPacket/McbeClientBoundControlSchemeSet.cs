

namespace Protocol.Network.MinecraftPacket;




public enum ControlSchemeType : byte
{
    
    
    
    LockedPlayerRelativeStrafe = 0,

    
    
    
    CameraRelative = 1,

    
    
    
    CameraRelativeStrafe = 2,

    
    
    
    PlayerRelative = 3,

    
    
    
    PlayerRelativeStrafe = 4
}





public class McpeClientBoundControlSchemeSet : Packet
{
    
    
    
    public McpeClientBoundControlSchemeSet()
    {
        Id = 327; 
        IsMcpe = true;
    }

    
    
    
    public byte ControlScheme { get; set; } 

    
    
    
    protected override void EncodePacket()
    {
        base.EncodePacket();

        
        Write(ControlScheme);
    }

    
    
    
    protected override void DecodePacket()
    {
        base.DecodePacket();

        
        ControlScheme = ReadByte();
    }

    
    
    
    protected override void ResetPacket()
    {
        base.ResetPacket();
        ControlScheme = 0;
    }
}