using Protocol.Minecraft;





namespace Protocol.Network.MinecraftPacket;





public class McpeClientCheatAbility : Packet
{
    
    
    
    public McpeClientCheatAbility()
    {
        Id = 197; 
        IsMcpe = true;
    }

    
    
    
    public AbilityData AbilityData { get; set; } = new(); 

    
    
    
    protected override void EncodePacket()
    {
        base.EncodePacket();

        
        
        
        WriteAbilityData(AbilityData);
    }

    
    
    
    protected override void DecodePacket()
    {
        base.DecodePacket();

        
        
        
        AbilityData = ReadAbilityData();
    }

    
    
    
    protected override void ResetPacket()
    {
        base.ResetPacket();
        AbilityData = new AbilityData(); 
    }

    #region 补全的方法 (因为 methods.txt 中没有 AbilityData 和 AbilityLayer 的直接读写)

    
    
    
    
    
    private void WriteAbilityData(AbilityData data)
    {
        if (data == null)
        {
            
            Write(0L); 
            Write((byte)0); 
            Write((byte)0); 
            WriteUnsignedVarInt(0); 
            
            return;
        }

        
        Write(data.EntityUniqueID);

        
        Write(data.PlayerPermissions);

        
        Write(data.CommandPermissions);

        
        
        WriteUnsignedVarInt((uint)(data.Layers?.Length ?? 0)); 
        if (data.Layers != null)
            foreach (var layer in data.Layers)
                
                
                Write(layer);
    }

    
    
    
    
    
    private AbilityData ReadAbilityData()
    {
        
        var entityUniqueID = ReadLong(); 

        
        var playerPermissions = ReadByte(); 

        
        var commandPermissions = ReadByte(); 

        
        
        var layersCount = ReadUnsignedVarInt(); 
        var layers = new AbilityLayer[layersCount];
        for (var i = 0; i < layersCount; i++)
            
            
            layers[i] = ReadAbilityLayer(); 

        return new AbilityData(entityUniqueID, playerPermissions, commandPermissions, layers);
    }

    #endregion
}