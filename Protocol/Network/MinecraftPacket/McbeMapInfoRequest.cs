using Protocol.Utils;

namespace Protocol.Network.MinecraftPacket;

public class McpeMapInfoRequest : Packet
{
	public long mapId;
	public pixelList pixellist;

	public McpeMapInfoRequest()
	{
		Id = 0x44;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteSignedVarLong(mapId);
		WriteUnsignedVarInt(0);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		mapId = ReadSignedVarLong();
		pixellist = ReadPixelList();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		mapId = default;
		pixellist = default;
	}
}