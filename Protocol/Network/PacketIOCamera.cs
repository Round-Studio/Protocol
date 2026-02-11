using System;
using System.Collections.Generic;
using System.Text;
using Protocol.Minecraft;
using Protocol.Minecraft.Camera;

namespace Protocol.Network
{
	public partial class Packet
	{
		
		public void Write(CameraAimAssistActorPriorityData value)
		{
			Write(value.PresetIndex);
			Write(value.CategoryIndex);
			Write(value.ActorIndex);
			Write(value.Priority);
		}

		public CameraAimAssistActorPriorityData ReadCameraAimAssistActorPriorityData()
		{
			return new CameraAimAssistActorPriorityData
			{
				PresetIndex = ReadInt(),
				CategoryIndex = ReadInt(),
				ActorIndex = ReadInt(),
				Priority = ReadInt()
			};
		}
		public void Write(CameraProgressOption value)
		{
			Write(value.Value);
			Write(value.Time);
			Write(value.EaseType.HasValue);
			if (value.EaseType.HasValue)
			{
				Write(value.EaseType.Value);
			}
		}

		public CameraProgressOption ReadCameraProgressOption()
		{
			var result = new CameraProgressOption
			{
				Value = ReadFloat(),
				Time = ReadFloat()
			};

			var hasEaseType = ReadBool();
			if (hasEaseType)
			{
				result.EaseType = new Optional<byte>(ReadByte());
			}

			return result;
		}

		public void Write(CameraRotationOption value)
		{
			Write(value.Value);
			Write(value.time);
		}

		public CameraRotationOption ReadCameraRotationOption()
		{
			return new CameraRotationOption
			{
				Value = ReadVector3(),
				time = ReadFloat()
			};
		}

		public void Write(CameraSplineInstruction value)
		{
			Write(value.TotalTime);

			Write(value.SplineType.HasValue);
			if (value.SplineType.HasValue)
			{
				Write(value.SplineType.Value);
			}

			WriteSlice(value.Curve.ToArray(), Write);
			WriteSlice(value.ProgressKeyFrames.ToArray(), Write);
			WriteSlice(value.RotationOptions.ToArray(), Write);
		}

		public CameraSplineInstruction ReadCameraSplineInstruction()
		{
			var instruction = new CameraSplineInstruction
			{
				TotalTime = ReadFloat()
			};

			var hasSplineType = ReadBool();
			if (hasSplineType)
			{
				instruction.SplineType = new Optional<byte>(ReadByte());
			}

			instruction.Curve = new System.Collections.Generic.List<System.Numerics.Vector3>(
				ReadSlice(ReadVector3));
			instruction.ProgressKeyFrames = new System.Collections.Generic.List<CameraProgressOption>(
				ReadSlice(ReadCameraProgressOption));
			instruction.RotationOptions = new System.Collections.Generic.List<CameraRotationOption>(
				ReadSlice(ReadCameraRotationOption));

			return instruction;
		}
		public void Write(CameraSplineDefinition value)
		{
			Write(value.Name);
			Write(value.Instruction);
		}

		public CameraSplineDefinition ReadCameraSplineDefinition()
		{
			return new CameraSplineDefinition
			{
				Name = ReadString(),
				Instruction = ReadCameraSplineInstruction()
			};
		}
	}
}