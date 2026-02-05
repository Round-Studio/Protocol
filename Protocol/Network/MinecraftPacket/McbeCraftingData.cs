using Protocol.Minecraft;


namespace Protocol.Network.MinecraftPacket;

public class McpeCraftingData : Packet
{
	public bool isClean;
	public MaterialReducerRecipe[] materialReducerRecipes;
	public PotionContainerChangeRecipe[] potionContainerRecipes;
	public PotionTypeRecipe[] potionTypeRecipes;

	public Recipes recipes;

	public McpeCraftingData()
	{
		Id = 0x34;
		IsMcpe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(recipes);
		Write(potionTypeRecipes);
		Write(potionContainerRecipes);
		Write(materialReducerRecipes);
		Write(isClean);
	}

	protected override void DecodePacket()
	{
		base.DecodePacket();


		recipes = ReadRecipes();
		potionTypeRecipes = ReadPotionTypeRecipes();
		potionContainerRecipes = ReadPotionContainerChangeRecipes();
		materialReducerRecipes = ReadMaterialReducerRecipes();
		isClean = ReadBool();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		recipes = default;
		potionTypeRecipes = default;
		potionContainerRecipes = default;
		materialReducerRecipes = default;
		isClean = default;
	}
}