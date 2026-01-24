

namespace Protocol.Network.MinecraftPacket;




public class McpePlayerUpdateEntityOverrides : Packet
{
    public enum PlayerUpdate : byte
    {
        
        
        
        PlayerUpdateEntityOverridesTypeClearAll = 0,

        
        
        
        PlayerUpdateEntityOverridesTypeRemove = 1,

        
        
        
        PlayerUpdateEntityOverridesTypeInt = 2,

        
        
        
        PlayerUpdateEntityOverridesTypeFloat = 3
    }

    
    
    
    public McpePlayerUpdateEntityOverrides()
    {
        Id = 325; 
        IsMcpe = true;
    }

    
    
    
    
    public long EntityRuntimeID { get; set; } 

    
    
    
    public uint PropertyIndex { get; set; } 

    
    
    
    public byte Type { get; set; } 

    
    
    
    public int IntValue { get; set; } 

    
    
    
    public float FloatValue { get; set; } 

    
    
    
    protected override void EncodePacket()
    {
        base.EncodePacket();

        
        WriteUnsignedVarLong(EntityRuntimeID);

        
        WriteUnsignedVarInt(PropertyIndex);

        
        Write(Type);

        if (Type == (byte)PlayerUpdate.PlayerUpdateEntityOverridesTypeInt)
            
            
            Write(IntValue);
        else if (Type == (byte)PlayerUpdate.PlayerUpdateEntityOverridesTypeFloat)
            
            Write(FloatValue);
        
        
    }

    
    
    
    protected override void DecodePacket()
    {
        base.DecodePacket();

        
        EntityRuntimeID = ReadUnsignedVarLong();

        
        PropertyIndex = ReadUnsignedVarInt();

        
        Type = ReadByte();

        
        IntValue = 0;
        FloatValue = 0.0f;

        if (Type == (byte)PlayerUpdate.PlayerUpdateEntityOverridesTypeInt)
            
            
            IntValue = ReadInt();
        else if (Type == (byte)PlayerUpdate.PlayerUpdateEntityOverridesTypeFloat)
            
            FloatValue = ReadFloat();
        
    }

    
    
    
    protected override void ResetPacket()
    {
        base.ResetPacket();
        EntityRuntimeID = 0;
        PropertyIndex = 0;
        Type = 0;
        IntValue = 0;
        FloatValue = 0.0f;
    }
}