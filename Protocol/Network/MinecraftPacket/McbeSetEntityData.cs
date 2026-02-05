using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McpeSetEntityData : Packet
{
	public MetadataDictionary metadata;

	public long runtimeEntityId;
	public PropertySyncData syncdata;
	public long tick;

	public McpeSetEntityData()
	{
		Id = 0x27;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteUnsignedVarLong(runtimeEntityId);
		Write(metadata);
		Write(syncdata);
		WriteUnsignedVarLong(tick);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		runtimeEntityId = ReadUnsignedVarLong();
		metadata = ReadMetadataDictionary();
		syncdata = ReadPropertySyncData();
		tick = ReadUnsignedVarLong();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		runtimeEntityId = default;
		metadata = default;
		syncdata = default;
		tick = default;
	}
}