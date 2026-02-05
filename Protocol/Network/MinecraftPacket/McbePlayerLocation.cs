using System.Numerics;


namespace Protocol.Network.MinecraftPacket;

public class McpePlayerLocation : Packet
{
	public enum PlayerLocation
	{
		PlayerLocationTypeCoordinates = 0,


		PlayerLocationTypeHide = 1
	}


	public McpePlayerLocation()
	{
		Id = 326;
		IsMcpe = true;
	}


	public int Type { get; set; }


	public long EntityUniqueID { get; set; }


	public Vector3 Position { get; set; }


	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(Type);


		WriteSignedVarLong(EntityUniqueID);

		if (Type == (int)PlayerLocation.PlayerLocationTypeCoordinates)

			Write(Position);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		Type = ReadInt();


		EntityUniqueID = ReadSignedVarLong();

		if (Type == (int)PlayerLocation.PlayerLocationTypeCoordinates)

			Position = ReadVector3();
		else


			Position = Vector3.Zero;
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();
		Type = 0;
		EntityUniqueID = 0;
		Position = Vector3.Zero;
	}
}