using System.Numerics;

namespace Protocol.Network.MinecraftPacket;

public class McpeChangeDimension : Packet
{
	public int dimension;
	public Vector3 position;
	public bool respawn;

	public McpeChangeDimension()
	{
		Id = 0x3d;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteSignedVarInt(dimension);
		Write(position);
		Write(respawn);
		Write(false);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		dimension = ReadSignedVarInt();
		position = ReadVector3();
		respawn = ReadBool();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		dimension = default;
		position = default;
		respawn = default;
	}
}