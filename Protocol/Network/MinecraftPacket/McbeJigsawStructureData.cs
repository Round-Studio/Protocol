using Protocol.Minecraft;


namespace Protocol.Network.MinecraftPacket;

public class McpeJigsawStructureData : Packet
{
	public McpeJigsawStructureData()
	{
		Id = 313;
		IsMcpe = true;


		StructureData = null;
	}


	public Nbt StructureData { get; set; }


	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(StructureData);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		StructureData = ReadNbt();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();


		StructureData = null;
	}
}