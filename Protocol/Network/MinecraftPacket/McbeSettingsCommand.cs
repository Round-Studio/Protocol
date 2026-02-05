namespace Protocol.Network.MinecraftPacket;

public class McpeSettingsCommand : Packet
{
	public McpeSettingsCommand()
	{
		Id = 140;
		IsMcpe = true;
	}


	public string CommandLine { get; set; }


	public bool SuppressOutput { get; set; }

	protected override void EncodePacket()
	{
		base.EncodePacket();
		Write(CommandLine);
		Write(SuppressOutput);
	}

	protected override void DecodePacket()
	{
		base.DecodePacket();
		CommandLine = ReadString();
		SuppressOutput = ReadBool();
	}
}