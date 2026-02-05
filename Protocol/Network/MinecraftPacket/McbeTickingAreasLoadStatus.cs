namespace Protocol.Network.MinecraftPacket;

public class McpeTickingAreasLoadStatus : Packet
{
	public McpeTickingAreasLoadStatus()
	{
		Id = 179;
		IsMcpe = true;
	}


	public bool Preload { get; set; }


	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(Preload);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		Preload = ReadBool();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();
		Preload = false;
	}
}