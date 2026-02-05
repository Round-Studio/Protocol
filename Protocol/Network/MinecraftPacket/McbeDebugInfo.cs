namespace Protocol.Network.MinecraftPacket;

public class McpeDebugInfo : Packet
{
	public McpeDebugInfo()
	{
		Id = 155;
		IsMcpe = true;
	}


	public long PlayerUniqueID { get; set; }


	public byte[] Data { get; set; } = new byte[0];


	protected override void EncodePacket()
	{
		base.EncodePacket();

		WriteSignedVarLong(PlayerUniqueID);
		WriteByteArray(Data);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();

		PlayerUniqueID = ReadSignedVarLong();
		Data = ReadByteArray();
	}
}