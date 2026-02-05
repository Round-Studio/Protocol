using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McpeUpdateAttributes : Packet
{
	public PlayerAttributes attributes;

	public long runtimeEntityId;
	public long tick;

	public McpeUpdateAttributes()
	{
		Id = 0x1d;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteUnsignedVarLong(runtimeEntityId);
		Write(attributes);
		WriteUnsignedVarLong(tick);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		runtimeEntityId = ReadUnsignedVarLong();
		attributes = ReadPlayerAttributes();
		tick = ReadUnsignedVarLong();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		runtimeEntityId = default;
		attributes = default;
		tick = default;
	}
}