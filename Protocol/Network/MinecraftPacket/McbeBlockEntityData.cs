using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McpeBlockEntityData : Packet
{
	public BlockCoordinates coordinates;
	public Nbt namedtag;

	public McpeBlockEntityData()
	{
		Id = 0x38;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(coordinates);
		Write(namedtag);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		coordinates = ReadBlockCoordinates();
		namedtag = ReadNbt();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		coordinates = default;
		namedtag = default;
	}
}