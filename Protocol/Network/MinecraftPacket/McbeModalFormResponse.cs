namespace Protocol.Network.MinecraftPacket;

public class McpeModalFormResponse : Packet
{
	public byte cancelReason;
	public string data = "";

	public uint formId;

	public McpeModalFormResponse()
	{
		Id = 0x65;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();
		WriteUnsignedVarInt(formId);
		Write(data);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		formId = ReadUnsignedVarInt();
		if (ReadBool()) data = ReadString();
		if (ReadBool()) cancelReason = ReadByte();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		formId = default;
		data = default;
		cancelReason = default;
	}
}