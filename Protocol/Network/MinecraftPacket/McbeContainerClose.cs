namespace Protocol.Network.MinecraftPacket;

public class McpeContainerClose : Packet
{
	public bool server;

	public byte windowId;

	public McpeContainerClose()
	{
		Id = 0x2f;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(windowId);
		Write((byte)0);
		Write(server);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		windowId = ReadByte();
		ReadByte();
		server = ReadBool();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		windowId = default;
		server = default;
	}
}