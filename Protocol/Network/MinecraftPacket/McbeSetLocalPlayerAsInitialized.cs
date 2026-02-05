namespace Protocol.Network.MinecraftPacket;

public class McpeSetLocalPlayerAsInitialized : Packet
{
	public long runtimeEntityId;

	public McpeSetLocalPlayerAsInitialized()
	{
		Id = 0x71;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteUnsignedVarLong(runtimeEntityId);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		runtimeEntityId = ReadUnsignedVarLong();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		runtimeEntityId = default;
	}
}