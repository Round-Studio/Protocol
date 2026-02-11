namespace Protocol.Network.MinecraftPacket;

public class McbeTakeItemEntity : Packet
{
	public long runtimeEntityId;
	public long target;

	public McbeTakeItemEntity()
	{
		Id = 0x11;
		IsMcbe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteUnsignedVarLong(runtimeEntityId);
		WriteUnsignedVarLong(target);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		runtimeEntityId = ReadUnsignedVarLong();
		target = ReadUnsignedVarLong();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		runtimeEntityId = default;
		target = default;
	}
}