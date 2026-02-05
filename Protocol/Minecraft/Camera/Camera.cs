using System.Drawing;
using System.Numerics;

namespace Protocol.Minecraft;

using System;
using System.Numerics;

public struct CameraEase
{
	public byte Type;


	public float Duration;
}

public struct CameraInstruction
{
	public uint Preset;


	public Optional<CameraEase> Ease;


	public Optional<Vector3> Position;


	public Optional<Vector2> Rotation;


	public Optional<Vector3> Facing;


	public Optional<Vector2> ViewOffset;


	public Optional<Vector3> EntityOffset;


	public Optional<bool> Default;


	public bool IgnoreStartingValuesComponent;
}

public struct CameraFadeTimeData
{
	public float FadeInDuration;


	public float WaitDuration;


	public float FadeOutDuration;
}

public struct CameraInstructionTarget
{
	public Optional<Vector3> CenterOffset;


	public long EntityUniqueID;
}

public struct CameraInstructionFieldOfView
{
	public float FieldOfView;


	public float EaseTime;


	public byte EaseType;


	public bool Clear;
}

public struct CameraInstructionFade
{
	public Optional<CameraFadeTimeData> TimeData;


	public Optional<Color> Colour;
}

public enum SplineEaseType
{
	SplineEaseTypeCatmullRom,
	SplineEaseTypeLinear
}

public struct CameraRotationOption
{
	public Vector3 Value { get; set; }
	public float time { get; set; }
}

public struct CameraSplineInstruction
{
	public float TotalTime;


	public byte EaseType;


	public Vector3[] Curve;


	public Vector2[] ProgressKeyFrames;


	public CameraRotationOption[] RotationOptions;
}

public struct CameraAimAssistPriority
{
	public string Identifier;


	public int Priority;

	public CameraAimAssistPriority(string identifier, int priority)
	{
		Identifier = identifier;
		Priority = priority;
	}
}

public struct CameraAimAssistPriorities
{
	public CameraAimAssistPriority[] Entities;


	public CameraAimAssistPriority[] Blocks;


	public Optional<int> EntityDefault;


	public Optional<int> BlockDefault;

	public CameraAimAssistPriorities(
		CameraAimAssistPriority[] entities,
		CameraAimAssistPriority[] blocks,
		Optional<int> entityDefault,
		Optional<int> blockDefault)
	{
		Entities = entities;
		Blocks = blocks;
		EntityDefault = entityDefault;
		BlockDefault = blockDefault;
	}
}

public struct CameraAimAssistCategory
{
	public string Name;


	public CameraAimAssistPriorities Priorities;

	public CameraAimAssistCategory(string name, CameraAimAssistPriorities priorities)
	{
		Name = name;
		Priorities = priorities;
	}
}

public struct CameraAimAssistItemSettings
{
	public string Item;


	public string Category;

	public CameraAimAssistItemSettings(string item, string category)
	{
		Item = item;
		Category = category;
	}
}

public partial struct CameraAimAssistPreset
{
	public string Identifier;


	public string[] BlockExclusions;


	public string[] LiquidTargets;


	public CameraAimAssistItemSettings[] ItemSettings;


	public Optional<string> DefaultItemSettings;


	public Optional<string> HandSettings;
}