namespace Protocol.Network.MinecraftPacket;

public class McpeRiderJump : Packet
{
	public int unknown;

	public McpeRiderJump()
	{
		Id = 0x14;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteSignedVarInt(unknown);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		unknown = ReadSignedVarInt();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		unknown = default;
	}
}