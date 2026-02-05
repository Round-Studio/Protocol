using Protocol.Network.MinecraftPacket;

namespace Protocol.Minecraft.Transaction;

public class CraftTransactionRecord : TransactionRecord
{
	public McpeInventoryTransaction.CraftingAction Action { get; set; }
}