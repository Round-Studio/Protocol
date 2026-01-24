using System.Numerics;
using Protocol.Utils;






namespace Protocol.Network.MinecraftPacket;





public class McpeClientMovementPredictionSync : Packet
{
    
    
    private const int EntityDataFlagCount = 125; 

    
    
    
    public McpeClientMovementPredictionSync()
    {
        Id = 322; 
        IsMcpe = true;
        
    }

    
    
    
    public Bitset ActorFlags { get; set; } = new(EntityDataFlagCount, BigInteger.Zero); 

    
    
    
    public float BoundingBoxScale { get; set; } 

    
    
    
    public float BoundingBoxWidth { get; set; } 

    
    
    
    public float BoundingBoxHeight { get; set; } 

    
    
    
    public float MovementSpeed { get; set; } 

    
    
    
    public float UnderwaterMovementSpeed { get; set; } 

    
    
    
    public float LavaMovementSpeed { get; set; } 

    
    
    
    public float JumpStrength { get; set; } 

    
    
    
    public float Health { get; set; } 

    
    
    
    public float Hunger { get; set; } 

    
    
    
    public long EntityUniqueID { get; set; } 

    
    
    
    public bool Flying { get; set; } 

    
    
    
    protected override void EncodePacket()
    {
        base.EncodePacket();

        
        
        WriteBitset(ActorFlags, EntityDataFlagCount);

        
        Write(BoundingBoxScale);
        Write(BoundingBoxWidth);
        Write(BoundingBoxHeight);
        Write(MovementSpeed);
        Write(UnderwaterMovementSpeed);
        Write(LavaMovementSpeed);
        Write(JumpStrength);
        Write(Health);
        Write(Hunger);

        
        WriteSignedVarLong(EntityUniqueID);

        
        Write(Flying);
    }

    
    
    
    protected override void DecodePacket()
    {
        base.DecodePacket();

        
        
        ActorFlags = ReadBitset(EntityDataFlagCount);

        
        BoundingBoxScale = ReadFloat();
        BoundingBoxWidth = ReadFloat();
        BoundingBoxHeight = ReadFloat();
        MovementSpeed = ReadFloat();
        UnderwaterMovementSpeed = ReadFloat();
        LavaMovementSpeed = ReadFloat();
        JumpStrength = ReadFloat();
        Health = ReadFloat();
        Hunger = ReadFloat();

        
        EntityUniqueID = ReadSignedVarLong();

        
        Flying = ReadBool();
    }

    
    
    
    protected override void ResetPacket()
    {
        base.ResetPacket();
        
        ActorFlags = new Bitset(EntityDataFlagCount, BigInteger.Zero);
        BoundingBoxScale = 0.0f;
        BoundingBoxWidth = 0.0f;
        BoundingBoxHeight = 0.0f;
        MovementSpeed = 0.0f;
        UnderwaterMovementSpeed = 0.0f;
        LavaMovementSpeed = 0.0f;
        JumpStrength = 0.0f;
        Health = 0.0f;
        Hunger = 0.0f;
        EntityUniqueID = 0;
        Flying = false;
    }
}