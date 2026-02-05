using Protocol.Minecraft;


namespace Protocol.Network.MinecraftPacket;

public class McpeEditorNetwork : Packet
{
	public McpeEditorNetwork()
	{
		Id = 190;
		IsMcpe = true;
	}


	public bool RouteToManager { get; set; }


	public Nbt
		Payload { get; set; }


	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(RouteToManager);


		Write(Payload);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		RouteToManager = ReadBool();


		Payload = ReadNbt();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();
		RouteToManager = false;


		Payload = null;
	}
}