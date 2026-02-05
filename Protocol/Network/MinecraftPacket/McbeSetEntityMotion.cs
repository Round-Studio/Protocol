using System.Numerics;

namespace Protocol.Network.MinecraftPacket;

public class McpeSetEntityMotion : Packet
{
	public long runtimeEntityId;
	public long tick;
	public Vector3 velocity;

	public McpeSetEntityMotion()
	{
		Id = 0x28;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteUnsignedVarLong(runtimeEntityId);
		Write(velocity);
		WriteUnsignedVarLong(tick);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		runtimeEntityId = ReadUnsignedVarLong();
		velocity = ReadVector3();
		tick = ReadUnsignedVarLong();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		runtimeEntityId = default;
		velocity = default;
		tick = default;
	}
}