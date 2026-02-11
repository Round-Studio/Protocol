using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McbeLevelEventGeneric : Packet
{
	public Nbt eventData;

	public int eventId;

	public McbeLevelEventGeneric()
	{
		Id = 0x7c;
		IsMcbe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteSignedVarInt(eventId);
		Write(eventData);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		eventId = ReadSignedVarInt();

		for (byte i = 0; i < 60; i++)
			ReadByte();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		eventId = default;
		eventData = default;
	}
}