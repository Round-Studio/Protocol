using System.Numerics;

namespace Protocol.Network.MinecraftPacket;

public class McpeInteract : Packet
{
    public enum Actions
    {
        RightClick = 1,
        LeftClick = 2,
        LeaveVehicle = 3,
        MouseOver = 4,
        OpenNpc = 5,
        OpenInventory = 6
    }

    public byte actionId; 
    public Vector3 Position;
    public long targetRuntimeEntityId; 


    public McpeInteract()
    {
        Id = 0x21;
        IsMcpe = true;
    }

    protected override void EncodePacket()
    {
        base.EncodePacket();


        Write(actionId);
        WriteUnsignedVarLong(targetRuntimeEntityId);

        if (actionId == (int)Actions.MouseOver || actionId == (int)Actions.LeaveVehicle)
            
            Write(Position);
    }


    protected override void DecodePacket()
    {
        base.DecodePacket();


        actionId = ReadByte();
        targetRuntimeEntityId = ReadUnsignedVarLong();
        if (actionId == (int)Actions.MouseOver || actionId == (int)Actions.LeaveVehicle)
            
            Position = ReadVector3();
    }


    protected override void ResetPacket()
    {
        base.ResetPacket();

        actionId = default;
        targetRuntimeEntityId = default;
    }
}