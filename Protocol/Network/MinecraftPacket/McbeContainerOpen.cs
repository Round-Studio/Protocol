using Protocol.Minecraft;


namespace Protocol.Network.MinecraftPacket;

public class McpeContainerOpen : Packet
{
	public BlockCoordinates coordinates;
	public long runtimeEntityId;
	public byte type;

	public byte windowId;

	public McpeContainerOpen()
	{
		Id = 0x2e;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(windowId);
		Write(type);
		Write(coordinates);
		WriteSignedVarLong(runtimeEntityId);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		windowId = ReadByte();
		type = ReadByte();
		coordinates = ReadBlockCoordinates();
		runtimeEntityId = ReadSignedVarLong();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		windowId = default;
		type = default;
		coordinates = default;
		runtimeEntityId = default;
	}
}