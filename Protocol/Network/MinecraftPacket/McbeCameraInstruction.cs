using Protocol.Minecraft;

namespace Protocol.Network.MinecraftPacket;

public class McpeCameraInstruction : Packet
{
	public McpeCameraInstruction()
	{
		Id = 300;
		IsMcpe = true;
	}


	public Optional<CameraAimAssistCategory>
		Set { get; set; }


	public Optional<bool> Clear { get; set; }


	public Optional<CameraAimAssistPreset>
		Fade { get; set; }


	public Optional<CameraAimAssistItemSettings>
		Target { get; set; }


	public Optional<bool> RemoveTarget { get; set; }


	public Optional<CameraAimAssistPriorities>
		FieldOfView { get; set; }

	public Optional<CameraSplineInstruction> Spline { get; set; }
	public Optional<long> AttachToEntity { get; set; }
	public Optional<bool> DetachFromEntity { get; set; }


	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(Set.HasValue);
		if (Set.HasValue) Write(Set.Value);


		Write(Clear.HasValue);
		if (Clear.HasValue) Write(Clear.Value);


		Write(Fade.HasValue);
		if (Fade.HasValue) Write(Fade.Value);


		Write(Target.HasValue);
		if (Target.HasValue) Write(Target.Value);


		Write(RemoveTarget.HasValue);
		if (RemoveTarget.HasValue) Write(RemoveTarget.Value);


		Write(FieldOfView.HasValue);
		if (FieldOfView.HasValue) Write(FieldOfView.Value);
		if (Spline.HasValue)
		{
			WriteCameraSplineInstruction(Spline.Value);
		}

		if (AttachToEntity.HasValue)
		{
			Write(AttachToEntity.Value);
		}

		if (DetachFromEntity.HasValue)
		{
			Write(DetachFromEntity.Value);
		}
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		var hasSet = ReadBool();
		if (hasSet) Set = new Optional<CameraAimAssistCategory>(ReadCameraAimAssistCategory());


		var hasClear = ReadBool();
		if (hasClear) Clear = new Optional<bool>(ReadBool());


		var hasFade = ReadBool();
		if (hasFade) Fade = new Optional<CameraAimAssistPreset>(ReadCameraAimAssistPreset());


		var hasTarget = ReadBool();
		if (hasTarget) Target = new Optional<CameraAimAssistItemSettings>(ReadCameraAimAssistItemSettings());


		var hasRemoveTarget = ReadBool();
		if (hasRemoveTarget) RemoveTarget = new Optional<bool>(ReadBool());


		var hasFieldOfView = ReadBool();
		if (hasFieldOfView) FieldOfView = new Optional<CameraAimAssistPriorities>(ReadCameraAimAssistPriorities());
		var hasFieldOfCamera = ReadBool();
		if (hasFieldOfCamera)
		{
			Spline = new Optional<CameraSplineInstruction>(ReadCameraSplineInstruction());
		}

		if (ReadBool())
		{
			AttachToEntity = new Optional<long>(ReadLong());
		}

		if (ReadBool())
		{
			DetachFromEntity = new Optional<bool>(ReadBool());
		}
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();
		Set = new Optional<CameraAimAssistCategory>();
		Clear = new Optional<bool>();
		Fade = new Optional<CameraAimAssistPreset>();
		Target = new Optional<CameraAimAssistItemSettings>();
		RemoveTarget = new Optional<bool>();
		FieldOfView = new Optional<CameraAimAssistPriorities>();
		Spline = new Optional<CameraSplineInstruction>();
		AttachToEntity = new Optional<long>();
		DetachFromEntity = new Optional<bool>();
	}

	#region Helper Read Methods for Complex Structs

	private CameraAimAssistPriority ReadCameraAimAssistPriority()
	{
		var identifier = ReadString();
		var priority = ReadInt();
		return new CameraAimAssistPriority(identifier, priority);
	}

	private void Write(CameraAimAssistPriority priority)
	{
		Write(priority.Identifier ?? string.Empty);
		Write(priority.Priority);
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

	private void Write(CameraAimAssistPriorities priorities)
	{
		WriteUnsignedVarInt((uint)(priorities.Entities?.Length ?? 0));
		if (priorities.Entities != null)
			foreach (var priority in priorities.Entities)
				Write(priority);


		WriteUnsignedVarInt((uint)(priorities.Blocks?.Length ?? 0));
		if (priorities.Blocks != null)
			foreach (var priority in priorities.Blocks)
				Write(priority);


		Write(priorities.EntityDefault.HasValue);
		if (priorities.EntityDefault.HasValue) Write(priorities.EntityDefault.Value);


		Write(priorities.BlockDefault.HasValue);
		if (priorities.BlockDefault.HasValue) Write(priorities.BlockDefault.Value);
	}


	private CameraAimAssistCategory ReadCameraAimAssistCategory()
	{
		var name = ReadString();
		var priorities = ReadCameraAimAssistPriorities();
		return new CameraAimAssistCategory(name, priorities);
	}

	private void Write(CameraAimAssistCategory category)
	{
		Write(category.Name ?? string.Empty);
		Write(category.Priorities);
	}


	private CameraAimAssistItemSettings ReadCameraAimAssistItemSettings()
	{
		var item = ReadString();
		var category = ReadString();
		return new CameraAimAssistItemSettings(item, category);
	}

	private void Write(CameraAimAssistItemSettings itemSettings)
	{
		Write(itemSettings.Item ?? string.Empty);
		Write(itemSettings.Category ?? string.Empty);
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
		if (preset.DefaultItemSettings.HasValue) Write(preset.DefaultItemSettings.Value ?? string.Empty);


		Write(preset.HandSettings.HasValue);
		if (preset.HandSettings.HasValue) Write(preset.HandSettings.Value ?? string.Empty);
	}

	#endregion
}