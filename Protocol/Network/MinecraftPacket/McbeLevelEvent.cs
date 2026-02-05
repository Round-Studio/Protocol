using System.Numerics;

namespace Protocol.Network.MinecraftPacket;

public class McpeLevelEvent : Packet
{
	public int data;

	public int eventId;
	public Vector3 position;

	public McpeLevelEvent()
	{
		Id = 0x19;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteSignedVarInt(eventId);
		Write(position);
		WriteSignedVarInt(data);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		eventId = ReadSignedVarInt();
		position = ReadVector3();
		data = ReadSignedVarInt();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		eventId = default;
		position = default;
		data = default;
	}
}