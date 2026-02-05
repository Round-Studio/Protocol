namespace Protocol.Network.MinecraftPacket;

public class McpeRequestChunkRadius : Packet
{
	public int chunkRadius;
	public byte maxRadius;

	public McpeRequestChunkRadius()
	{
		Id = 0x45;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteSignedVarInt(chunkRadius);
		Write(maxRadius);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		chunkRadius = ReadSignedVarInt();
		maxRadius = ReadByte();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		chunkRadius = default;
		maxRadius = default;
	}
}