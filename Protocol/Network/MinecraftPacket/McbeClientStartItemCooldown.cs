namespace Protocol.Network.MinecraftPacket;

public class McpeClientStartItemCooldown : Packet
{
	public McpeClientStartItemCooldown()
	{
		Id = 176;
		IsMcpe = true;
	}


	public string Category { get; set; } = string.Empty;


	public int Duration { get; set; }


	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(Category);


		WriteSignedVarInt(Duration);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		Category = ReadString();


		Duration = ReadSignedVarInt();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();
		Category = string.Empty;
		Duration = 0;
	}
}