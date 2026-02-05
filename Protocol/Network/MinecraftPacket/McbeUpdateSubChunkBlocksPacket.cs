using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McpeUpdateSubChunkBlocksPacket : Packet
{
	public UpdateSubChunkBlocksPacketEntry[] layerOneUpdates;
	public UpdateSubChunkBlocksPacketEntry[] layerZeroUpdates;

	public BlockCoordinates subchunkCoordinates;

	public McpeUpdateSubChunkBlocksPacket()
	{
		Id = 0xac;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(subchunkCoordinates);
		Write(layerZeroUpdates);
		Write(layerOneUpdates);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		subchunkCoordinates = ReadBlockCoordinates();
		layerZeroUpdates = ReadUpdateSubChunkBlocksPacketEntrys();
		layerOneUpdates = ReadUpdateSubChunkBlocksPacketEntrys();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		subchunkCoordinates = default;
		layerZeroUpdates = default;
		layerOneUpdates = default;
	}
}