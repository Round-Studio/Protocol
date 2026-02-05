using Protocol.Minecraft;
using Protocol.Utils;

namespace Protocol.Network.MinecraftPacket;

public class McpeResourcePacksInfo : Packet
{
	public bool ForceDisableVibrantVisuals;
	public bool hasAddons;
	public bool hasScripts;
	public bool mustAccept;
	public UUID templateUUID;
	public string templateVersion;
	public TexturePackInfos texturepacks;

	public McpeResourcePacksInfo()
	{
		Id = 0x06;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(mustAccept);
		Write(hasAddons);
		Write(hasScripts);
		Write(ForceDisableVibrantVisuals);
		Write(templateUUID);
		Write(templateVersion);
		Write(texturepacks);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		mustAccept = ReadBool();
		hasAddons = ReadBool();
		hasScripts = ReadBool();
		ForceDisableVibrantVisuals = ReadBool();
		templateUUID = ReadUUID();
		templateVersion = ReadString();
		texturepacks = ReadTexturePackInfos();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		mustAccept = default;
		hasAddons = default;
		hasScripts = default;
		ForceDisableVibrantVisuals = default;
		templateUUID = default;
		templateVersion = default;
		texturepacks = default;
	}
}