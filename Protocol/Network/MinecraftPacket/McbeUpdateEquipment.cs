using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McpeUpdateEquipment : Packet
{
	public long entityId;
	public Nbt namedtag;
	public byte unknown;

	public byte windowId;
	public byte windowType;

	public McpeUpdateEquipment()
	{
		Id = 0x51;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(windowId);
		Write(windowType);
		Write(unknown);
		WriteSignedVarLong(entityId);
		Write(namedtag);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		windowId = ReadByte();
		windowType = ReadByte();
		unknown = ReadByte();
		entityId = ReadSignedVarLong();
		namedtag = ReadNbt();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		windowId = default;
		windowType = default;
		unknown = default;
		entityId = default;
		namedtag = default;
	}
}