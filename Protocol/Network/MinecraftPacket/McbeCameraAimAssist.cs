using System.Numerics;




namespace Protocol.Network.MinecraftPacket;




public enum CameraAimAssistAction : byte
{
    
    
    
    Set = 0,

    
    
    
    Clear = 1
}




public class McpeCameraAimAssist : Packet
{
    
    
    
    public McpeCameraAimAssist()
    {
        Id = 316; 
        IsMcpe = true;
    }
    
    
    
    
    


    
    
    
    public string Preset { get; set; } = string.Empty; 

    
    
    
    
    public Vector2 Angle { get; set; } 

    
    
    
    public float Distance { get; set; } 

    
    
    
    public byte TargetMode { get; set; } 

    
    
    
    public CameraAimAssistAction Action { get; set; } 

    
    
    
    public bool ShowDebugRender { get; set; } 

    
    
    
    protected override void EncodePacket()
    {
        base.EncodePacket();

        
        Write(Preset);

        
        Write(Angle);

        
        Write(Distance);

        
        Write(TargetMode);

        
        
        Write((byte)Action);

        
        Write(ShowDebugRender);
    }

    
    
    
    protected override void DecodePacket()
    {
        base.DecodePacket();

        
        Preset = ReadString();

        
        Angle = ReadVector2();

        
        Distance = ReadFloat();

        
        TargetMode = ReadByte();

        
        
        Action = (CameraAimAssistAction)ReadByte();

        
        ShowDebugRender = ReadBool();
    }

    
    
    
    protected override void ResetPacket()
    {
        base.ResetPacket();
        Preset = string.Empty;
        Angle = Vector2.Zero;
        Distance = 0.0f;
        TargetMode = 0; 
        Action = CameraAimAssistAction.Set; 
        ShowDebugRender = false;
    }
}