using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McpeItemComponent : Packet
{
	public Itemstates entries;

	public McpeItemComponent()
	{
		Id = 0xa2;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(entries);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		entries = ReadItemstates();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		entries = default;
	}
}