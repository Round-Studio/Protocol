namespace Protocol.Network.MinecraftPacket;

public static class NPCDialogueActionType
{
	public const int Open = 0;


	public const int Close = 1;
}

public class McpeNPCDialogue : Packet
{
	public McpeNPCDialogue()
	{
		Id = 169;
		IsMcpe = true;
	}


	public ulong EntityUniqueID { get; set; }


	public int ActionType { get; set; }


	public string Dialogue { get; set; } = string.Empty;


	public string SceneName { get; set; } = string.Empty;


	public string NPCName { get; set; } = string.Empty;


	public string ActionJSON { get; set; } = string.Empty;


	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(EntityUniqueID);


		WriteSignedVarInt(ActionType);


		Write(Dialogue);


		Write(SceneName);


		Write(NPCName);


		Write(ActionJSON);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		EntityUniqueID = ReadUlong();


		ActionType = ReadSignedVarInt();


		Dialogue = ReadString();


		SceneName = ReadString();


		NPCName = ReadString();


		ActionJSON = ReadString();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();
		EntityUniqueID = 0;
		ActionType = NPCDialogueActionType.Open;

		Dialogue = string.Empty;
		SceneName = string.Empty;
		NPCName = string.Empty;
		ActionJSON = string.Empty;
	}
}