

namespace Protocol.Network.MinecraftPacket;





public class McpeServerBoundDiagnostics : Packet
{
    
    
    
    public McpeServerBoundDiagnostics()
    {
        Id = 315; 
        IsMcpe = true;
    }

    
    
    
    public float AverageFramesPerSecond { get; set; } 

    
    
    
    public float AverageServerSimTickTime { get; set; } 

    
    
    
    public float AverageClientSimTickTime { get; set; } 

    
    
    
    public float AverageBeginFrameTime { get; set; } 

    
    
    
    public float AverageInputTime { get; set; } 

    
    
    
    public float AverageRenderTime { get; set; } 

    
    
    
    public float AverageEndFrameTime { get; set; } 

    
    
    
    public float AverageRemainderTimePercent { get; set; } 

    
    
    
    public float AverageUnaccountedTimePercent { get; set; } 

    
    
    
    protected override void EncodePacket()
    {
        base.EncodePacket();

        
        Write(AverageFramesPerSecond);
        Write(AverageServerSimTickTime);
        Write(AverageClientSimTickTime);
        Write(AverageBeginFrameTime);
        Write(AverageInputTime);
        Write(AverageRenderTime);
        Write(AverageEndFrameTime);
        Write(AverageRemainderTimePercent);
        Write(AverageUnaccountedTimePercent);
    }

    
    
    
    protected override void DecodePacket()
    {
        base.DecodePacket();

        
        AverageFramesPerSecond = ReadFloat();
        AverageServerSimTickTime = ReadFloat();
        AverageClientSimTickTime = ReadFloat();
        AverageBeginFrameTime = ReadFloat();
        AverageInputTime = ReadFloat();
        AverageRenderTime = ReadFloat();
        AverageEndFrameTime = ReadFloat();
        AverageRemainderTimePercent = ReadFloat();
        AverageUnaccountedTimePercent = ReadFloat();
    }

    
    
    
    protected override void ResetPacket()
    {
        base.ResetPacket();
        AverageFramesPerSecond = 0.0f;
        AverageServerSimTickTime = 0.0f;
        AverageClientSimTickTime = 0.0f;
        AverageBeginFrameTime = 0.0f;
        AverageInputTime = 0.0f;
        AverageRenderTime = 0.0f;
        AverageEndFrameTime = 0.0f;
        AverageRemainderTimePercent = 0.0f;
        AverageUnaccountedTimePercent = 0.0f;
    }
}