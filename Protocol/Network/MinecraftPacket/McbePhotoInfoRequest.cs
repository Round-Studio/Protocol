namespace Protocol.Network.MinecraftPacket;

public class McpePhotoInfoRequest : Packet
{
	public McpePhotoInfoRequest()
	{
		Id = 173;
		IsMcpe = true;
	}


	public long PhotoID { get; set; }


	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteSignedVarLong(PhotoID);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		PhotoID = ReadSignedVarLong();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();
		PhotoID = 0;
	}
}