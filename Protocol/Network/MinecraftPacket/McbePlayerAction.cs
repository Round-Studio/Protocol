using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McpePlayerAction : Packet
{
	public int actionId;
	public BlockCoordinates coordinates;
	public int face;
	public BlockCoordinates resultCoordinates;

	public long runtimeEntityId;

	public McpePlayerAction()
	{
		Id = 0x24;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteUnsignedVarLong(runtimeEntityId);
		WriteSignedVarInt(actionId);
		Write(coordinates);
		Write(resultCoordinates);
		WriteSignedVarInt(face);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		runtimeEntityId = ReadUnsignedVarLong();
		actionId = ReadSignedVarInt();
		coordinates = ReadBlockCoordinates();
		resultCoordinates = ReadBlockCoordinates();
		face = ReadSignedVarInt();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		runtimeEntityId = default;
		actionId = default;
		coordinates = default;
		resultCoordinates = default;
		face = default;
	}
}