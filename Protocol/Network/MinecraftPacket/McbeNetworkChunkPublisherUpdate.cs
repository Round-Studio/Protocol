using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McbeNetworkChunkPublisherUpdate : Packet
{
	public BlockCoordinates coordinates;
	public uint radius;
	public int savedChunks;
	public uint x;
	public uint z;

	public McbeNetworkChunkPublisherUpdate()
	{
		Id = 0x79;
		IsMcbe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(coordinates);
		WriteUnsignedVarInt(radius);
		Write(savedChunks);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		coordinates = ReadBlockCoordinates();
		radius = ReadUnsignedVarInt();
		savedChunks = ReadInt();
		for (var i = 0; i < savedChunks; i++)
		{
			x = ReadUnsignedVarInt();
			z = ReadUnsignedVarInt();
		}
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		coordinates = default;
		radius = default;
		savedChunks = default;
		x = default(int);
		z = default(int);
	}
}