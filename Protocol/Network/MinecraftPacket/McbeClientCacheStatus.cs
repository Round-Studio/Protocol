namespace Protocol.Network.MinecraftPacket;

public class McpeClientCacheStatus : Packet
{
	public bool enabled;

	public McpeClientCacheStatus()
	{
		Id = 0x81;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(enabled);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		enabled = ReadBool();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		enabled = default;
	}
}