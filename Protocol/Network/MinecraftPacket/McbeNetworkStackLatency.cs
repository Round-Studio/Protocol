namespace Protocol.Network.MinecraftPacket;

public class McpeNetworkStackLatency : Packet
{
	public ulong timestamp;
	public byte unknownFlag;

	public McpeNetworkStackLatency()
	{
		Id = 0x73;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(timestamp);
		Write(unknownFlag);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		timestamp = ReadUlong();
		unknownFlag = ReadByte();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		timestamp = default;
		unknownFlag = default;
	}
}