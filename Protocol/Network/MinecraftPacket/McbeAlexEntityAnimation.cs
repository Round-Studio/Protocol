using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McbeAlexEntityAnimation : Packet
{
	public string boneId;
	public AnimationKey[] keys;

	public long runtimeEntityId;

	public McbeAlexEntityAnimation()
	{
		Id = 0xe0;
		IsMcbe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteUnsignedVarLong(runtimeEntityId);
		Write(boneId);
		Write(keys);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		runtimeEntityId = ReadUnsignedVarLong();
		boneId = ReadString();
		keys = ReadAnimationKeys();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		runtimeEntityId = default;
		boneId = default;
		keys = default;
	}
}