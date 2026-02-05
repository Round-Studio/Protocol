namespace Protocol.Network.MinecraftPacket;

public class McpeSetInventoryOptions : Packet
{
	public int craftingLayout;
	public bool filtering;
	public int inventoryLayout;

	public int leftTab;
	public int rightTab;

	public McpeSetInventoryOptions()
	{
		Id = 0x133;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		leftTab = ReadSignedVarInt();
		rightTab = ReadSignedVarInt();
		filtering = ReadBool();
		inventoryLayout = ReadSignedVarInt();
		craftingLayout = ReadSignedVarInt();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		leftTab = default;
		rightTab = default;
		filtering = default;
		inventoryLayout = default;
		craftingLayout = default;
	}
}