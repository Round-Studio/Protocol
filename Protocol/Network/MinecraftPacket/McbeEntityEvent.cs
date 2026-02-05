namespace Protocol.Network.MinecraftPacket;

public class McpeEntityEvent : Packet
{
	public int data;
	public byte eventId;

	public long runtimeEntityId;

	public McpeEntityEvent()
	{
		Id = 0x1b;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteUnsignedVarLong(runtimeEntityId);
		Write(eventId);
		WriteSignedVarInt(data);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		runtimeEntityId = ReadUnsignedVarLong();
		eventId = ReadByte();
		data = ReadSignedVarInt();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		runtimeEntityId = default;
		eventId = default;
		data = default;
	}
}