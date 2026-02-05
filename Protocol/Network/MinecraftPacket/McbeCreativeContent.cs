using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McpeCreativeContent : Packet
{
	public List<creativeGroup> groups;
	public List<CreativeItemEntry> input;

	public McpeCreativeContent()
	{
		Id = 0x91;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(groups);
		Write(input);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		groups = ReadCreativeGroups();
		input = ReadCreativeItemStacks();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		groups = default;
		input = default;
	}
}