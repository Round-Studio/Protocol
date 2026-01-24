using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Protocol.Minecraft.Graphic;

namespace Protocol.Network.MinecraftPacket
{
	public class McbeGraphicsOverrideParameter : Packet
	{
		 public	ParameterKeyframeValue[] parameterKeyframeValues;
		 public string BiomeIdentifier;
		 public byte ParameterType;
		 public bool reset;
		public McbeGraphicsOverrideParameter()
		{
			IsMcpe = true;
			Id = 331;
		}

		protected override void DecodePacket()
		{
			base.DecodePacket();
			var length = ReadUnsignedVarInt();

			for (int i = 0; i < length; i++)
			{
				parameterKeyframeValues[i] = ReadParameterKeyframeValue();
			}

			BiomeIdentifier = ReadString();
			ParameterType = ReadByte();
			reset = ReadBool();
		}

		protected override void EncodePacket()
		{
			base.EncodePacket();
			var length = parameterKeyframeValues.Length;
			for (int i = 0; i < length; i++)
			{
				Write(parameterKeyframeValues[i]);
			}
			Write(BiomeIdentifier);
			Write(ParameterType);
			Write(reset);
		}

		protected override void ResetPacket()
		{
			base.ResetPacket();
			
		}
	}
}
