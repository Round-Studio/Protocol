using Protocol.Minecraft;
using Protocol.Minecraft.Transaction;

namespace Protocol.Network.MinecraftPacket;

public class McpeInventorySlot : Packet
{
	public FullContainerName ContainerName = new();

	public uint inventoryId;
	public Item item;
	public uint slot;
	public Item storageItem;

	public McpeInventorySlot()
	{
		Id = 0x32;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteUnsignedVarInt(inventoryId);
		WriteUnsignedVarInt(slot);
		Write(ContainerName);
		Write(storageItem);
		Write(item);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		inventoryId = ReadUnsignedVarInt();
		slot = ReadUnsignedVarInt();
		ContainerName = readFullContainerName();
		storageItem = ReadItem();
		item = ReadItem();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		inventoryId = default;
		slot = default;
		ContainerName = default;
		storageItem = default;
		item = default;
	}
}