namespace Protocol.Network.MinecraftPacket;

public class McbeMobEffect : Packet
{
	public int amplifier;
	public int duration;
	public int effectId;
	public byte eventId;
	public bool particles;

	public long runtimeEntityId;
	public long tick;

	public McbeMobEffect()
	{
		Id = 0x1c;
		IsMcbe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteUnsignedVarLong(runtimeEntityId);
		Write(eventId);
		WriteSignedVarInt(effectId);
		WriteSignedVarInt(amplifier);
		Write(particles);
		WriteSignedVarInt(duration);
		WriteUnsignedVarLong(tick);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		runtimeEntityId = ReadUnsignedVarLong();
		eventId = ReadByte();
		effectId = ReadSignedVarInt();
		amplifier = ReadSignedVarInt();
		particles = ReadBool();
		duration = ReadSignedVarInt();
		tick = ReadUnsignedVarLong();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		runtimeEntityId = default;
		eventId = default;
		effectId = default;
		amplifier = default;
		particles = default;
		duration = default;
		tick = default;
	}
}