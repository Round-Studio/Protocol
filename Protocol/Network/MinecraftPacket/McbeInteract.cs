using System.Numerics;

namespace Protocol.Network.MinecraftPacket;
public class McbeInteract : Packet
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
    public McbeInteract()
    {
        Id = 0x21;
        IsMcbe = true;
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
}