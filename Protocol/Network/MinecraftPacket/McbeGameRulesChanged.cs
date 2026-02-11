using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McbeGameRulesChanged : Packet
{
	public GameRules rules;

	public McbeGameRulesChanged()
	{
		Id = 0x48;
		IsMcbe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(rules);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		rules = ReadGameRules();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		rules = default;
	}
}