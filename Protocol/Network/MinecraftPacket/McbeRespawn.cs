namespace Protocol.Network.MinecraftPacket;

public class McpeRespawn : Packet
{
	public enum RespawnState
	{
		Search = 0,
		Ready = 1,
		ClientReady = 2
	}

	public long runtimeEntityId;
	public byte state;

	public float x;
	public float y;
	public float z;

	public McpeRespawn()
	{
		Id = 0x2d;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(x);
		Write(y);
		Write(z);
		Write(state);
		WriteUnsignedVarLong(runtimeEntityId);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		x = ReadFloat();
		y = ReadFloat();
		z = ReadFloat();
		state = ReadByte();
		runtimeEntityId = ReadUnsignedVarLong();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		x = default;
		y = default;
		z = default;
		state = default;
		runtimeEntityId = default;
	}
}