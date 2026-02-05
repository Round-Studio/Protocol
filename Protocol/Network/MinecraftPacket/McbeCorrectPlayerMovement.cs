using System.Numerics;

namespace Protocol.Network.MinecraftPacket;

public class McpeCorrectPlayerMovement : Packet
{
	public bool OnGround;
	public Vector3 Postition;
	public long Tick;

	public byte Type;
	public Vector3 Velocity;

	public McpeCorrectPlayerMovement()
	{
		Id = 0xA1;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(Type);
		Write(Postition);
		Write(Velocity);
		Write(OnGround);
		WriteUnsignedVarLong(Tick);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		Type = ReadByte();
		Postition = ReadVector3();
		Velocity = ReadVector3();
		OnGround = ReadBool();
		Tick = ReadUnsignedVarLong();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		Type = default;
		Postition = default;
		Velocity = default;
		OnGround = default;
		Tick = default;
	}
}