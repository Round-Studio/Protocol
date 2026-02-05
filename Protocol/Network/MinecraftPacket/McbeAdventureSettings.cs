namespace Protocol.Network.MinecraftPacket;

public class McpeAdventureSettings : Packet
{
	public uint actionPermissions;
	public uint commandPermission;
	public uint customStoredPermissions;
	public long entityUniqueId;

	public uint flags;
	public uint permissionLevel;

	public McpeAdventureSettings()
	{
		Id = 0x37;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteUnsignedVarInt(flags);
		WriteUnsignedVarInt(commandPermission);
		WriteUnsignedVarInt(actionPermissions);
		WriteUnsignedVarInt(permissionLevel);
		WriteUnsignedVarInt(customStoredPermissions);
		Write(entityUniqueId);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		flags = ReadUnsignedVarInt();
		commandPermission = ReadUnsignedVarInt();
		actionPermissions = ReadUnsignedVarInt();
		permissionLevel = ReadUnsignedVarInt();
		customStoredPermissions = ReadUnsignedVarInt();
		entityUniqueId = ReadLong();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		flags = default;
		commandPermission = default;
		actionPermissions = default;
		permissionLevel = default;
		customStoredPermissions = default;
		entityUniqueId = default;
	}
}