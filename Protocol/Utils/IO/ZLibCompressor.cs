using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
using Protocol.Network;
using Protocol.Network.MinecraftPacket;

namespace Protocol.Utils.IO
{
	

	public static class ZLibHelper
	{
		private static List<Packet> ReadZlibPacket(ReadOnlyMemory<byte> payload)
		{
			var packets = new List<Packet>();

			using (var stream = new MemoryStreamReader(payload))
			{
				using (var deflateStream = new DeflateStream(stream, CompressionMode.Decompress, false))
				{
					using var s = new MemoryStream();
					deflateStream.CopyTo(s);
					s.Position = 0;

					int count = 0;
					// Get actual packet out of bytes
					while (s.Position < s.Length)
					{
						count++;

						uint len = VarInt.ReadUInt32(s);
						long pos = s.Position;
						ReadOnlyMemory<byte> internalBuffer = s.GetBuffer().AsMemory((int)s.Position, (int)len);
						int id = VarInt.ReadInt32(s);
						try
						{
							packets.Add(PacketFactory.translatePacket(id, internalBuffer, false) ??
							            new UnknownPacket((byte)id, internalBuffer));
						}
						catch (Exception e)
						{
							//Logger.warn($"Error parsing bedrock message #{count} id={id}\n{Packet.HexDump(internalBuffer)}");
							//throw;
							return packets; // Exit, but don't crash.
						}

						s.Position = pos + len;
					}

					if (s.Length > s.Position)
					{
						throw new Exception("Have more data");
					}
				}
			}

			return packets;
		}
		private static List<Packet> ReadNonePacket(ReadOnlyMemory<byte> payload)
		{
			var packets = new List<Packet>();

			using var stream = new MemoryStreamReader(payload);
			using var s = new MemoryStream();

			stream.CopyTo(s);
			s.Position = 0;

			int count = 0;
			while (s.Position < s.Length)
			{
				count++;

				uint len = VarInt.ReadUInt32(s);
				long pos = s.Position;
				ReadOnlyMemory<byte> internalBuffer = s.GetBuffer().AsMemory((int)s.Position, (int)len);
				int id = VarInt.ReadInt32(s);
				try
				{
					packets.Add(PacketFactory.translatePacket(id, internalBuffer, false) ??
					            new UnknownPacket((byte)id, internalBuffer));
				}
				catch (Exception e)
				{
					
					return packets; 
				}

				s.Position = pos + len;
			}

			if (s.Length > s.Position)
			{
				throw new Exception("Have more data");
			}

			return packets;
		}
		public static List<Packet> Decompress(ReadOnlyMemory<byte> payload,CompressionAlgorithm compressionAlgorithm,bool HasCompressid = false)
		{
			if (compressionAlgorithm == CompressionAlgorithm.None && HasCompressid == false)
			{
				return ReadNonePacket(payload);
			}

			return ReadZlibPacket(payload);
		}
		public static byte[] Compress(List<Packet> packets,CompressionAlgorithm compression = CompressionAlgorithm.None, CompressionLevel compressionLevel = CompressionLevel.Fastest)
		{
			long length = 0;
			foreach (Packet packet in packets)
			{
				length += packet.Encode().Length;
			}

			using (MemoryStream stream = Protocol.Utils.IO.MemoryStreamManger.stream.GetStream())
			{
				

				if (compression != CompressionAlgorithm.None)
				{
					stream.WriteByte((byte) compression);
				}

				if (length > 1000)
				{
					using (var compressStream = new DeflateStream(stream, compressionLevel, true))
					{
						foreach (Packet packet in packets)
						{
							byte[] bs = packet.Encode();
							if (bs != null && bs.Length > 0)
							{
								VarInt.WriteUInt32(compressStream, (uint)bs.Length);
								compressStream.Write(bs, 0, bs.Length);
							}

						}
					}
				}
				else
				{
					foreach (Packet packet in packets)
					{
						byte[] bs = packet.Encode();
						if (bs != null && bs.Length > 0)
						{
							VarInt.WriteUInt32(stream, (uint)bs.Length); ;
							stream.Write(bs, 0, bs.Length);
						}
					}
				}

				return stream.ToArray();
			}
		}
	}
	public enum CompressionAlgorithm
	{
		ZLib = 0,
		Snappy = 1,
		None = 255
	}
}
