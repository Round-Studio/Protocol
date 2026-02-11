using System.Numerics;

namespace Protocol.Network.MinecraftPacket;

public class McbeSetEntityMotion : Packet
{
	public long runtimeEntityId;
	public long tick;
	public Vector3 velocity;

	public McbeSetEntityMotion()
	{
		Id = 0x28;
		IsMcbe = true;
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