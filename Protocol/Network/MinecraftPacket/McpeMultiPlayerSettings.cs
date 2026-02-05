namespace Protocol.Network.MinecraftPacket;

public class McpeMultiPlayerSettings : Packet
{
	public enum Action
	{
		EnableMultiPlayer = 0,
		DisableMultiPlayer = 1,
		RefreshJoinCode = 2
	}

	public McpeMultiPlayerSettings()
	{
		Id = 139;
		IsMcpe = true;
	}


	public int ActionType { get; set; }

	protected override void EncodePacket()
	{
		base.EncodePacket();
		WriteSignedVarInt(ActionType);
	}

	protected override void DecodePacket()
	{
		base.DecodePacket();
		ActionType = ReadSignedVarInt();
	}
}