namespace Protocol.Network.MinecraftPacket;

public class McpeChangeMobProperty : Packet
{
	public McpeChangeMobProperty()
	{
		Id = 182;
		IsMcpe = true;
	}


	public ulong EntityUniqueID { get; set; }


	public string Property { get; set; } = string.Empty;


	public bool BoolValue { get; set; }


	public string StringValue { get; set; } = string.Empty;


	public int IntValue { get; set; }


	public float FloatValue { get; set; }


	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(EntityUniqueID);


		Write(Property);


		Write(BoolValue);


		Write(StringValue);


		WriteSignedVarInt(IntValue);


		Write(FloatValue);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		EntityUniqueID = ReadUlong();


		Property = ReadString();


		BoolValue = ReadBool();


		StringValue = ReadString();


		IntValue = ReadSignedVarInt();


		FloatValue = ReadFloat();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();
		EntityUniqueID = 0;
		Property = string.Empty;
		BoolValue = false;
		StringValue = string.Empty;
		IntValue = 0;
		FloatValue = 0.0f;
	}
}