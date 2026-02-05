namespace Protocol.Network.MinecraftPacket;

public class McpeCreatePhoto : Packet
{
	public McpeCreatePhoto()
	{
		Id = 171;
		IsMcpe = true;
	}


	public long EntityUniqueID { get; set; }


	public string PhotoName { get; set; } = string.Empty;


	public string ItemName { get; set; } = string.Empty;


	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(EntityUniqueID);


		Write(PhotoName);


		Write(ItemName);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		EntityUniqueID = ReadLong();


		PhotoName = ReadString();


		ItemName = ReadString();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();
		EntityUniqueID = 0;
		PhotoName = string.Empty;
		ItemName = string.Empty;
	}
}