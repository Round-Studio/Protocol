namespace Protocol.Network.MinecraftPacket;

public class McpeRefreshEntitlements : Packet
{
	public McpeRefreshEntitlements()
	{
		Id = 305;
		IsMcpe = true;
	}


	protected override void EncodePacket()
	{
		base.EncodePacket();
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();
	}
}