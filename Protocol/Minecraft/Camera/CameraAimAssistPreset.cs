namespace Protocol.Minecraft;

public partial struct CameraAimAssistPreset
{
	public CameraAimAssistPreset(
		string identifier,
		string[] blockExclusions,
		string[] liquidTargets,
		CameraAimAssistItemSettings[] itemSettings,
		Optional<string> defaultItemSettings,
		Optional<string> handSettings)
	{
		Identifier = identifier;
		BlockExclusions = blockExclusions;
		LiquidTargets = liquidTargets;
		ItemSettings = itemSettings;
		DefaultItemSettings = defaultItemSettings;
		HandSettings = handSettings;
	}
}