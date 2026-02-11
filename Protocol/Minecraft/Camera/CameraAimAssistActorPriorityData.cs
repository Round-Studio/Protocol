using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol.Minecraft.Camera
{
	public struct CameraAimAssistActorPriorityData
	{
		public int PresetIndex { get; set; }
		public int CategoryIndex { get; set; }
		public int ActorIndex { get; set; }
		public int Priority { get; set; }
	}
}
