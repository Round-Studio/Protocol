namespace Protocol.Network.MinecraftPacket;

public enum UnlockedRecipesType : uint
{
	Empty = 0,


	InitiallyUnlocked = 1,


	NewlyUnlocked = 2,


	RemoveUnlocked = 3,


	RemoveAllUnlocked = 4
}

public class McpeUnlockedRecipes : Packet
{
	public McpeUnlockedRecipes()
	{
		Id = 199;
		IsMcpe = true;
	}


	public UnlockedRecipesType UnlockType { get; set; }


	public string[] Recipes { get; set; } = new string[0];


	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write((uint)UnlockType);


		WriteUnsignedVarInt((uint)(Recipes?.Length ?? 0));

		if (Recipes != null)
			foreach (var recipe in Recipes)

				Write(recipe ?? string.Empty);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		UnlockType = (UnlockedRecipesType)ReadUint();


		var count = ReadUnsignedVarInt();

		Recipes = new string[count];
		for (var i = 0; i < count; i++)

			Recipes[i] = ReadString();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();
		UnlockType = UnlockedRecipesType.Empty;
		Recipes = new string[0];
	}
}