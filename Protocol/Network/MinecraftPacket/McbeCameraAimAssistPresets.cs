using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public enum CameraAimAssistPresetOperation : byte
{
	Set = 0,


	AddToExisting = 1
}

public class McpeCameraAimAssistPresets : Packet
{
	public McpeCameraAimAssistPresets()
	{
		Id = 320;
		IsMcpe = true;
	}


	public CameraAimAssistCategory[] Categories { get; set; } =
		new CameraAimAssistCategory[0];


	public CameraAimAssistPreset[] Presets { get; set; } =
		new CameraAimAssistPreset[0];


	public CameraAimAssistPresetOperation Operation { get; set; }


	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteUnsignedVarInt((uint)(Categories?.Length ?? 0));

		if (Categories != null)
			foreach (var category in Categories)
				Write(category);


		WriteUnsignedVarInt((uint)(Presets?.Length ?? 0));

		if (Presets != null)
			foreach (var preset in Presets)
				Write(preset);


		Write((byte)Operation);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		var categoriesCount = ReadUnsignedVarInt();

		Categories = new CameraAimAssistCategory[categoriesCount];
		for (var i = 0; i < categoriesCount; i++)
			Categories[i] = ReadCameraAimAssistCategory();


		var presetsCount = ReadUnsignedVarInt();

		Presets = new CameraAimAssistPreset[presetsCount];
		for (var i = 0; i < presetsCount; i++)
			Presets[i] = ReadCameraAimAssistPreset();


		Operation = (CameraAimAssistPresetOperation)ReadByte();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();
		Categories = new CameraAimAssistCategory[0];
		Presets = new CameraAimAssistPreset[0];
		Operation = CameraAimAssistPresetOperation.Set;
	}


	private void Write(CameraAimAssistCategory category)
	{
		Write(category.Name ?? string.Empty);
		Write(category.Priorities);
	}


	private void Write(CameraAimAssistPriorities priorities)
	{
		WriteUnsignedVarInt((uint)(priorities.Entities?.Length ?? 0));
		if (priorities.Entities != null)
			foreach (var entityPriority in priorities.Entities)
				Write(entityPriority);


		WriteUnsignedVarInt((uint)(priorities.Blocks?.Length ?? 0));
		if (priorities.Blocks != null)
			foreach (var blockPriority in priorities.Blocks)
				Write(blockPriority);


		Write(priorities.EntityDefault.HasValue);
		if (priorities.EntityDefault.HasValue) Write(priorities.EntityDefault.Value);


		Write(priorities.BlockDefault.HasValue);
		if (priorities.BlockDefault.HasValue) Write(priorities.BlockDefault.Value);
	}


	private void Write(CameraAimAssistPriority priority)
	{
		Write(priority.Identifier ?? string.Empty);
		Write(priority.Priority);
	}


	private void Write(CameraAimAssistPreset preset)
	{
		Write(preset.Identifier ?? string.Empty);


		WriteUnsignedVarInt((uint)(preset.BlockExclusions?.Length ?? 0));
		if (preset.BlockExclusions != null)
			foreach (var exclusion in preset.BlockExclusions)
				Write(exclusion ?? string.Empty);


		WriteUnsignedVarInt((uint)(preset.LiquidTargets?.Length ?? 0));
		if (preset.LiquidTargets != null)
			foreach (var target in preset.LiquidTargets)
				Write(target ?? string.Empty);


		WriteUnsignedVarInt((uint)(preset.ItemSettings?.Length ?? 0));
		if (preset.ItemSettings != null)
			foreach (var itemSetting in preset.ItemSettings)
				Write(itemSetting);


		Write(preset.DefaultItemSettings.HasValue);
		if (preset.DefaultItemSettings.HasValue)
			Write(preset.DefaultItemSettings.Value ?? string.Empty);


		Write(preset.HandSettings.HasValue);
		if (preset.HandSettings.HasValue) Write(preset.HandSettings.Value ?? string.Empty);
	}


	private void Write(CameraAimAssistItemSettings itemSettings)
	{
		Write(itemSettings.Item ?? string.Empty);
		Write(itemSettings.Category ?? string.Empty);
	}


	private CameraAimAssistCategory ReadCameraAimAssistCategory()
	{
		var name = ReadString();
		var priorities = ReadCameraAimAssistPriorities();
		return new CameraAimAssistCategory(name, priorities);
	}


	private CameraAimAssistPriorities ReadCameraAimAssistPriorities()
	{
		var entitiesCount = ReadUnsignedVarInt();
		var entities = new CameraAimAssistPriority[entitiesCount];
		for (var i = 0; i < entitiesCount; i++) entities[i] = ReadCameraAimAssistPriority();


		var blocksCount = ReadUnsignedVarInt();
		var blocks = new CameraAimAssistPriority[blocksCount];
		for (var i = 0; i < blocksCount; i++) blocks[i] = ReadCameraAimAssistPriority();


		var hasEntityDefault = ReadBool();
		var entityDefault = new Optional<int>();
		if (hasEntityDefault) entityDefault = new Optional<int>(ReadInt());


		var hasBlockDefault = ReadBool();
		var blockDefault = new Optional<int>();
		if (hasBlockDefault) blockDefault = new Optional<int>(ReadInt());

		return new CameraAimAssistPriorities(entities, blocks, entityDefault, blockDefault);
	}


	private CameraAimAssistPriority ReadCameraAimAssistPriority()
	{
		var identifier = ReadString();
		var priority = ReadInt();
		return new CameraAimAssistPriority(identifier, priority);
	}


	private CameraAimAssistPreset ReadCameraAimAssistPreset()
	{
		var identifier = ReadString();


		var blockExclusionsCount = ReadUnsignedVarInt();
		var blockExclusions = new string[blockExclusionsCount];
		for (var i = 0; i < blockExclusionsCount; i++) blockExclusions[i] = ReadString();


		var liquidTargetsCount = ReadUnsignedVarInt();
		var liquidTargets = new string[liquidTargetsCount];
		for (var i = 0; i < liquidTargetsCount; i++) liquidTargets[i] = ReadString();


		var itemSettingsCount = ReadUnsignedVarInt();
		var itemSettings = new CameraAimAssistItemSettings[itemSettingsCount];
		for (var i = 0; i < itemSettingsCount; i++) itemSettings[i] = ReadCameraAimAssistItemSettings();


		var hasDefaultItemSettings = ReadBool();
		var defaultItemSettings = new Optional<string>();
		if (hasDefaultItemSettings) defaultItemSettings = new Optional<string>(ReadString());


		var hasHandSettings = ReadBool();
		var handSettings = new Optional<string>();
		if (hasHandSettings) handSettings = new Optional<string>(ReadString());

		return new CameraAimAssistPreset(identifier, blockExclusions, liquidTargets, itemSettings, defaultItemSettings,
			handSettings);
	}


	private CameraAimAssistItemSettings ReadCameraAimAssistItemSettings()
	{
		var item = ReadString();
		var category = ReadString();
		return new CameraAimAssistItemSettings(item, category);
	}
}