namespace Protocol.Network.MinecraftPacket;

public class McpeServerStats : Packet
{
	public McpeServerStats()
	{
		Id = 192;
		IsMcpe = true;
	}


	public float ServerTime { get; set; }


	public float NetworkTime { get; set; }


	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(ServerTime);


		Write(NetworkTime);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		ServerTime = ReadFloat();


		NetworkTime = ReadFloat();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();
		ServerTime = 0.0f;
		NetworkTime = 0.0f;
	}
}