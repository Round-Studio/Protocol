using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McbeEmoteList : Packet
{
	public EmoteIds emoteIds;

	public long runtimeEntityId;

	public McbeEmoteList()
	{
		Id = 0x8a;
		IsMcbe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteUnsignedVarLong(runtimeEntityId);
		Write(emoteIds);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		runtimeEntityId = ReadUnsignedVarLong();
		emoteIds = ReadEmoteId();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		runtimeEntityId = default;
		emoteIds = default;
	}
}