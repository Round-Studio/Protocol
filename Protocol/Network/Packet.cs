using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using fNbt;
using Microsoft.IO;
using Protocol.Minecraft;
using Protocol.Minecraft.Graphic;
using Protocol.Minecraft.Skins;
using Protocol.Minecraft.Transaction;
using Protocol.Network.MinecraftPacket;
using Protocol.Utils;
using Protocol.Utils.Crypto;
using Protocol.Utils.IO;
using Protocol.Utils.UDP;
using Protocol.Utils;
using Transaction = Protocol.Minecraft.Transaction.Transaction;
using Protocol.Minecraft.Map;
using ArgumentException = System.ArgumentException;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;
using BitConverter = System.BitConverter;
using Console = System.Console;
using DateTimeOffset = System.DateTimeOffset;
using Exception = System.Exception;
using Guid = System.Guid;
using InvalidOperationException = System.InvalidOperationException;
using Math = System.Math;


namespace Protocol.Network
{
	public abstract partial class Packet
	{
		private byte[] _encodedMessage;

		[JsonIgnore] public int Id;
		[JsonIgnore] public bool IsMcbe;

		protected MemoryStreamReader _reader;
		protected private Stream _buffer;
		private BinaryWriter _writer;

		[JsonIgnore] public System.ReadOnlyMemory<byte> Bytes { get; private set; }

		public Packet()
		{
		}
		

		public void WritePackSetting(PackSetting setting)
		{
			if (setting == null)
			{
				Write(string.Empty);
				uint defaultType = (int)PackSettingType.String;
				WriteUnsignedVarInt(defaultType);
				Write(string.Empty);
				return;
			}


			Write(setting.Name ?? string.Empty);


			uint typeId;
			if (setting.Value is float floatValue)
			{
				typeId = (int)PackSettingType.Float;
				WriteUnsignedVarInt(typeId);
				Write(floatValue);
			}
			else if (setting.Value is bool boolValue)
			{
				typeId = (int)PackSettingType.Bool;
				WriteUnsignedVarInt(typeId);
				Write(boolValue);
			}
			else if (setting.Value is string stringValue)
			{
				typeId = (int)PackSettingType.String;
				WriteUnsignedVarInt(typeId);
				Write(stringValue);
			}
			else
			{
				throw new ArgumentException(
					$"Unknown type for PackSetting.Value: {setting.Value?.GetType().Name ?? "null"}. Expected float, bool, or string.");
			}
		}

		public void Write(sbyte value)
		{
			_writer.Write(value);
		}

		public void Write(byte value)
		{
			_writer.Write(value);
		}

		public void Write(bool value)
		{
			Write((byte)(value ? 1 : 0));
		}


		public PackSetting ReadPackSetting()
		{
			var setting = new PackSetting();


			setting.Name = ReadString();


			uint typeId = ReadUnsignedVarInt();


			switch ((PackSettingType)typeId)
			{
				case PackSettingType.Float:
					float floatValue = ReadFloat();
					setting.Value = floatValue;
					break;
				case PackSettingType.Bool:
					bool boolValue = ReadBool();
					setting.Value = boolValue;
					break;
				case PackSettingType.String:
					string stringValue = ReadString();
					setting.Value = stringValue;
					break;
				default:


					throw new InvalidOperationException(
						$"Unknown PackSetting type ID: {typeId}. Expected {PackSettingType.Float}, {PackSettingType.Bool}, or {PackSettingType.String}.");
			}

			return setting;
		}


		public void WriteBitset(Bitset bitset, int size)
		{
			if (BigInteger.Zero == bitset.IntValue)
			{
				Write((byte)0x00);
				return;
			}


			BigInteger valueToWrite = BigInteger.Abs(bitset.IntValue);


			while (valueToWrite >= 0x80)
			{
				byte b = (byte)((valueToWrite & 0x7F) | 0x80);


				Write(b);


				valueToWrite >>= 7;
			}


			Write((byte)(valueToWrite & 0x7F));
		}


		public Bitset ReadBitset(int size)
		{
			BigInteger value = BigInteger.Zero;


			int shift = 0;
			byte b;


			do
			{
				b = ReadByte();


				BigInteger chunk = new BigInteger(b & 0x7F);
				value += (chunk << shift);


				shift += 7;
			} while ((b & 0x80) != 0);


			return new Bitset(size, value);
		}

		public sbyte ReadSByte()
		{
			return (sbyte)_reader.ReadByte();
		}

		public byte ReadByte()
		{
			return (byte)_reader.ReadByte();
		}

		public bool ReadBool()
		{
			return _reader.ReadByte() != 0;
		}

		public void Write(System.Memory<byte> value)
		{
			Write((System.ReadOnlyMemory<byte>)value);
		}

		public void Write(System.ReadOnlyMemory<byte> value)
		{
			if (value.IsEmpty)
			{
				return;
			}

			_writer.Write(value.Span);
		}

		public void Write(byte[] value)
		{
			if (value == null)
			{
				return;
			}

			_writer.Write(value);
		}

		public System.ReadOnlyMemory<byte> Slice(int count)
		{
			return _reader.Read(count);
		}

		public System.ReadOnlyMemory<byte> ReadReadOnlyMemory(int count, bool slurp = false)
		{
			if (!slurp && count == 0) return System.Memory<byte>.Empty;

			if (count == 0)
			{
				count = (int)(_reader.Length - _reader.Position);
			}

			System.ReadOnlyMemory<byte> readBytes = _reader.Read(count);
			if (readBytes.Length != count)
				throw new ArgumentOutOfRangeException($"Expected {count} bytes, only read {readBytes.Length}.");
			return readBytes;
		}

		public byte[] ReadBytes(int count, bool slurp = false)
		{
			if (!slurp && count == 0) return new byte[0];

			if (count == 0)
			{
				count = (int)(_reader.Length - _reader.Position);
			}

			System.ReadOnlyMemory<byte> readBytes = _reader.Read(count);
			if (readBytes.Length != count)
				throw new ArgumentOutOfRangeException($"Expected {count} bytes, only read {readBytes.Length}.");
			return readBytes.ToArray();
		}

		public void WriteByteArray(byte[] value)
		{
			if (value == null)
			{
				WriteLength(0);
				return;
			}

			WriteLength(value.Length);

			if (value.Length == 0) return;

			_writer.Write(value, 0, value.Length);
		}

		public byte[] ReadByteArray(bool slurp = false)
		{
			var len = ReadLength();
			var bytes = ReadBytes(len, slurp);
			return bytes;
		}

		public void Write(ulong[] value)
		{
			if (value == null)
			{
				WriteLength(0);
				return;
			}

			WriteLength(value.Length);

			if (value.Length == 0) return;
			for (int i = 0; i < value.Length; i++)
			{
				ulong val = value[i];
				Write(val);
			}
		}

		public ulong[] ReadUlongs(bool slurp = false)
		{
			var len = ReadLength();
			var ulongs = new ulong[len];
			for (int i = 0; i < ulongs.Length; i++)
			{
				ulongs[i] = ReadUlong();
			}

			return ulongs;
		}

		public void Write(short value, bool bigEndian = false)
		{
			if (bigEndian) _writer.Write(BinaryPrimitives.ReverseEndianness(value));
			else _writer.Write(value);
		}

		public short ReadShort(bool bigEndian = false)
		{
			if (_reader.Position == _reader.Length) return 0;

			if (bigEndian) return BinaryPrimitives.ReverseEndianness(_reader.ReadInt16());

			return _reader.ReadInt16();
		}

		public void Write(ushort value, bool bigEndian = false)
		{
			if (bigEndian) _writer.Write(BinaryPrimitives.ReverseEndianness(value));
			else _writer.Write(value);
		}

		public ushort ReadUshort(bool bigEndian = false)
		{
			if (_reader.Position == _reader.Length) return 0;

			if (bigEndian) return BinaryPrimitives.ReverseEndianness(_reader.ReadUInt16());

			return _reader.ReadUInt16();
		}

		public void WriteBe(short value)
		{
			_writer.Write(BinaryPrimitives.ReverseEndianness(value));
		}

		public short ReadShortBe()
		{
			if (_reader.Position == _reader.Length) return 0;

			return BinaryPrimitives.ReverseEndianness(_reader.ReadInt16());
		}

		public void Write(Int24 value)
		{
			_writer.Write(value.GetBytes());
		}

		public Int24 ReadLittle()
		{
			return new Int24(_reader.Read(3).Span);
		}

		public void Write(int value, bool bigEndian = false)
		{
			if (bigEndian) _writer.Write(BinaryPrimitives.ReverseEndianness(value));
			else _writer.Write(value);
		}

		public int ReadInt(bool bigEndian = false)
		{
			if (bigEndian) return BinaryPrimitives.ReverseEndianness(_reader.ReadInt32());

			return _reader.ReadInt32();
		}

		public void WriteBe(ushort value)
		{
			Write(value, true);
		}

		public void WriteBe(int value)
		{
			_writer.Write(BinaryPrimitives.ReverseEndianness(value));
		}

		public int ReadIntBe()
		{
			return BinaryPrimitives.ReverseEndianness(_reader.ReadInt32());
		}

		public void Write(uint value)
		{
			_writer.Write(value);
		}

		public uint ReadUint()
		{
			return _reader.ReadUInt32();
		}


		public void WriteVarInt(int value)
		{
			VarInt.WriteInt32(_buffer, value);
		}

		public int ReadVarInt()
		{
			return VarInt.ReadInt32(_reader);
		}

		public void WriteSignedVarInt(int value)
		{
			VarInt.WriteSInt32(_buffer, value);
		}

		public int ReadSignedVarInt()
		{
			return VarInt.ReadSInt32(_reader);
		}

		public void WriteUnsignedVarInt(uint value)
		{
			VarInt.WriteUInt32(_buffer, value);
		}

		public uint ReadUnsignedVarInt()
		{
			return VarInt.ReadUInt32(_reader);
		}

		public int ReadLength()
		{
			return (int)VarInt.ReadUInt32(_reader);
		}

		public void WriteLength(int value)
		{
			VarInt.WriteUInt32(_buffer, (uint)value);
		}

		public void WriteVarLong(long value)
		{
			VarInt.WriteInt64(_buffer, value);
		}

		public long ReadVarLong()
		{
			return VarInt.ReadInt64(_reader);
		}

		public void WriteEntityId(long value)
		{
			WriteSignedVarLong(value);
		}

		public void WriteSignedVarLong(long value)
		{
			VarInt.WriteSInt64(_buffer, value);
		}

		public long ReadSignedVarLong()
		{
			return VarInt.ReadSInt64(_reader);
		}

		public void WriteRuntimeEntityId(long value)
		{
			WriteUnsignedVarLong(value);
		}

		public void WriteUnsignedVarLong(long value)
		{
			VarInt.WriteUInt64(_buffer, (ulong)value);
		}

		public long ReadUnsignedVarLong()
		{
			return (long)VarInt.ReadUInt64(_reader);
		}

		public void Write(long value)
		{
			_writer.Write(BinaryPrimitives.ReverseEndianness(value));
		}

		public long ReadLong()
		{
			return BinaryPrimitives.ReverseEndianness(_reader.ReadInt64());
		}

		public void Write(ulong value)
		{
			_writer.Write(value);
		}

		public ulong ReadUlong()
		{
			return _reader.ReadUInt64();
		}

		public void Write(float value)
		{
			_writer.Write(value);
		}

		public float ReadFloat()
		{
			return _reader.ReadSingle();
		}

		public void Write(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				WriteLength(0);
				return;
			}

			byte[] bytes = Encoding.UTF8.GetBytes(value);

			WriteLength(bytes.Length);
			Write(bytes);
		}

		public string ReadString()
		{
			if (_reader.Position == _reader.Length) return string.Empty;
			int len = ReadLength();
			if (len <= 0) return string.Empty;
			return Encoding.UTF8.GetString(ReadBytes(len));
		}

		public void WriteFixedString(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				Write((short)0, true);
				return;
			}

			byte[] bytes = Encoding.UTF8.GetBytes(value);

			Write((short)bytes.Length, true);
			Write(bytes);
		}

		public string ReadFixedString()
		{
			if (_reader.Position == _reader.Length) return string.Empty;
			short len = ReadShort(true);
			if (len <= 0) return string.Empty;
			return Encoding.UTF8.GetString(_reader.Read(len).Span);
		}

		public void Write(Vector2 vec)
		{
			Write((float)vec.X);
			Write((float)vec.Y);
		}

		public Vector2 ReadVector2()
		{
			return new Vector2(ReadFloat(), ReadFloat());
		}

		public void Write(Vector3 vec)
		{
			Write((float)vec.X);
			Write((float)vec.Y);
			Write((float)vec.Z);
		}

		public Vector3 ReadVector3()
		{
			return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
		}


		public void Write(BlockCoordinates coord)
		{
			WriteSignedVarInt(coord.X);
			WriteUnsignedVarInt((uint)coord.Y);
			WriteSignedVarInt(coord.Z);
		}

		public void WritePaintingCoordinates(BlockCoordinates coord)
		{
			Write((float)coord.X);
			Write((float)coord.Y);
			Write((float)coord.Z);
		}

		public BlockCoordinates ReadBlockCoordinates()
		{
			return new BlockCoordinates(ReadSignedVarInt(), (int)ReadUnsignedVarInt(), ReadSignedVarInt());
		}

		public void Write(PlayerRecords records)
		{
			if (records is PlayerAddRecords)
			{
				Write((byte)0);
				WriteUnsignedVarInt((uint)records.Count);
				foreach (var record in records)
				{
					Write(record.ClientUuid);
					WriteSignedVarLong(record.EntityId);
					Write(record.DisplayName ?? record.Username);
					Write(record.PlayerInfo.CertificateData?.ExtraData?.Xuid ?? String.Empty);
					Write(record.PlayerInfo.PlatformChatId);
					Write(record.PlayerInfo.DeviceOS);
					Write(record.Skin);
					Write(false);
					Write(false);
					Write(false);
					Write(0);
				}
			}
			else if (records is PlayerRemoveRecords)
			{
				Write((byte)1);
				WriteUnsignedVarInt((uint)records.Count);
				foreach (var record in records)
				{
					Write(record.ClientUuid);
				}
			}

			if (records is PlayerAddRecords)
			{
				foreach (var record in records)
				{
					Write(record.Skin.IsVerified);
				}
			}
		}

		public PlayerRecords ReadPlayerRecords()
		{
			byte recordType = ReadByte();
			uint count = ReadUnsignedVarInt();
			PlayerRecords records = null;
			switch (recordType)
			{
				case 0:
					records = new PlayerAddRecords();
					for (int i = 0; i < count; i++)
					{
						var player = new Player(null, null);
						player.ClientUuid = ReadUUID();
						player.EntityId = ReadSignedVarLong();
						player.DisplayName = ReadString();
						var xuid = ReadString();
						var platformChatId = ReadString();
						var deviceOS = ReadInt();
						player.Skin = ReadSkin();
						ReadBool();
						ReadBool();
						ReadBool();
						ReadInt();

						player.PlayerInfo = new PlayerInfo()
						{
							PlatformChatId = platformChatId,
							DeviceOS = deviceOS,
							CertificateData = new CertificateData()
							{
								ExtraData = new ExtraData()
								{
									Xuid = xuid
								}
							}
						};
						records.Add(player);
					}

					break;
				case 1:
					records = new PlayerRemoveRecords();
					for (int i = 0; i < count; i++)
					{
						var player = new Player(null, null);
						player.ClientUuid = ReadUUID();
						records.Add(player);
					}

					break;
			}

			if (records is PlayerAddRecords)
			{
				foreach (Player player in records)
				{
					bool isVerified = ReadBool();

					if (player.Skin != null)
						player.Skin.IsVerified = isVerified;
				}
			}


			return records;
		}

		public void Write(Records records)
		{
			WriteUnsignedVarInt((uint)records.Count);
			foreach (BlockCoordinates coord in records)
			{
				Write(coord);
			}
		}

		public Records ReadRecords()
		{
			var records = new Records();
			uint count = ReadUnsignedVarInt();
			for (int i = 0; i < count; i++)
			{
				var coord = ReadBlockCoordinates();
				records.Add(coord);
			}

			return records;
		}

		public void Write(PlayerLocation location)
		{
			Write(location.X);
			Write(location.Y);
			Write(location.Z);
			var d = 256f / 360f;
			Write((byte)Math.Round(location.Pitch * d));
			Write((byte)Math.Round(location.HeadYaw * d));
			Write((byte)Math.Round(location.Yaw * d));
		}

		public PlayerLocation ReadPlayerLocation()
		{
			PlayerLocation location = new PlayerLocation();
			location.X = ReadFloat();
			location.Y = ReadFloat();
			location.Z = ReadFloat();
			location.Pitch = ReadByte() * 1f / 0.71f;
			location.HeadYaw = ReadByte() * 1f / 0.71f;
			location.Yaw = ReadByte() * 1f / 0.71f;

			return location;
		}

		public void Write(IPEndPoint endpoint)
		{
			if (endpoint.AddressFamily == AddressFamily.InterNetwork)
			{
				Write((byte)4);
				var parts = endpoint.Address.ToString().Split('.');
				foreach (var part in parts)
				{
					Write((byte)~byte.Parse(part));
				}

				Write((short)endpoint.Port, true);
			}
		}


		public IPEndPoint ReadIPEndPoint()
		{
			byte ipVersion = ReadByte();

			IPAddress address = IPAddress.Any;
			int port = 0;

			if (ipVersion == 4)
			{
				string ipAddress = $"{(byte)~ReadByte()}.{(byte)~ReadByte()}.{(byte)~ReadByte()}.{(byte)~ReadByte()}";
				address = IPAddress.Parse(ipAddress);
				port = (ushort)ReadShort(true);
			}
			else if (ipVersion == 6)
			{
				ReadShort();
				port = (ushort)ReadShort(true);
				ReadLong();
				var addressBytes = ReadBytes(16);
				address = new IPAddress(addressBytes);
			}
			else
			{
			}

			return new IPEndPoint(address, port);
		}

		public void Write(IPEndPoint[] endpoints)
		{
			foreach (var endpoint in endpoints)
			{
				Write(endpoint);
			}
		}

		public IPEndPoint[] ReadIPEndPoints(int count)
		{
			if (count == 20 && _reader.Length < 120) count = 10;
			var endPoints = new IPEndPoint[count];
			for (int i = 0; i < endPoints.Length; i++)
			{
				endPoints[i] = ReadIPEndPoint();
			}

			return endPoints;
		}

		public void Write(UUID uuid)
		{
			if (uuid == null) throw new Exception("Expected UUID, required");
			Write(uuid.GetBytes());
		}

		public UUID ReadUUID()
		{
			UUID uuid = new UUID(ReadBytes(16));
			return uuid;
		}

		public void Write(Nbt nbt)
		{
			Write(nbt, _writer.BaseStream,
				nbt.NbtFile.UseVarInt || this is McbeBlockEntityData || this is McbeUpdateEquipment);
		}

		public static void Write(Nbt nbt, Stream stream, bool useVarInt)
		{
			NbtFile file = nbt.NbtFile;
			file.BigEndian = false;
			file.UseVarInt = useVarInt;

			byte[] saveToBuffer = file.SaveToBuffer(NbtCompression.None);
			stream.Write(saveToBuffer, 0, saveToBuffer.Length);
		}


		public Nbt ReadNbt()
		{
			return ReadNbt(_reader);
		}

		public static Nbt ReadNbt(Stream stream, bool allowAlternativeRootTag = true, bool useVarInt = true)
		{
			Nbt nbt = new Nbt();
			NbtFile nbtFile = new NbtFile();
			nbtFile.BigEndian = false;
			nbtFile.UseVarInt = useVarInt;
			nbtFile.AllowAlternativeRootTag = allowAlternativeRootTag;

			nbt.NbtFile = nbtFile;
			nbtFile.LoadFromStream(stream, NbtCompression.AutoDetect);

			return nbt;
		}

		public static NbtCompound ReadNbtCompound(Stream stream, bool useVarInt = false)
		{
			NbtFile file = new NbtFile();
			file.BigEndian = false;
			file.UseVarInt = useVarInt;
			file.AllowAlternativeRootTag = false;

			file.LoadFromStream(stream, NbtCompression.None);

			return (NbtCompound)file.RootTag;
		}

		public void Write(MetadataInts metadata)
		{
			if (metadata == null)
			{
				WriteUnsignedVarInt(0);
				return;
			}

			WriteUnsignedVarInt((uint)metadata.Count);

			for (byte i = 0; i < metadata.Count; i++)
			{
				MetadataInt slot = metadata[i] as MetadataInt;
				if (slot != null)
				{
					WriteUnsignedVarInt((uint)slot.Value);
				}
			}
		}

		public MetadataInts ReadMetadataInts()
		{
			MetadataInts metadata = new MetadataInts();
			uint count = ReadUnsignedVarInt();

			for (byte i = 0; i < count; i++)
			{
				metadata[i] = new MetadataInt((int)ReadUnsignedVarInt());
			}

			return metadata;
		}

		public void Write(List<CreativeItemEntry> itemStacks)
		{
			WriteUnsignedVarInt((uint)itemStacks.Count);

			var netId = 0;
			foreach (var item in itemStacks)
			{
				item.Item.RuntimeId = 0;
				WriteUnsignedVarInt((uint)netId);
				Write(item.Item, false);
				WriteUnsignedVarInt(item.GroupIndex);
				netId++;
			}
		}

		public List<CreativeItemEntry> ReadCreativeItemStacks()
		{
			var metadata = new List<CreativeItemEntry>();

			var count = ReadUnsignedVarInt();
			for (int i = 0; i < count; i++)
			{
				var networkId = ReadUnsignedVarInt();
				Item item = ReadItem(false);
				item.NetworkId = (int)networkId;
				uint groupIndex = ReadUnsignedVarInt();
				metadata.Add(new CreativeItemEntry(groupIndex, item));
			}

			return metadata;
		}

		public void Write(List<creativeGroup> groups)
		{
			WriteUnsignedVarInt((uint)groups.Count);

			foreach (var group in groups)
			{
				Write(group.Category);
				Write(group.Name);
				Write(group.Icon, false);
			}
		}

		public List<creativeGroup> ReadCreativeGroups()
		{
			var group = new List<creativeGroup>();

			var groupCount = ReadUnsignedVarInt();
			for (int i = 0; i < groupCount; i++)
			{
				int category = ReadInt();
				string name = ReadString();
				Item item = ReadItem(false);
				group.Add(new creativeGroup(category, name, item));
			}

			return group;
		}

		public void Write(ItemStacks itemStacks)
		{
			if (itemStacks == null)
			{
				WriteUnsignedVarInt(0);
				return;
			}

			WriteUnsignedVarInt((uint)itemStacks.Count);
			for (int i = 0; i < itemStacks.Count; i++)
			{
				Write(itemStacks[i]);
			}
		}

		public ItemStacks ReadItemStacks()
		{
			var metadata = new ItemStacks();

			var count = ReadUnsignedVarInt();
			for (int i = 0; i < count; i++)
			{
				int networkId = 0;
				if (this is McbeCreativeContent) networkId = ReadVarInt();
				Item item = ReadItem(this is not McbeCreativeContent);
				item.NetworkId = networkId;
				metadata.Add(item);
			}

			return metadata;
		}

		public void Write(Transaction transaction)
		{
			WriteSignedVarInt(transaction.RequestId);

			if (transaction.RequestId != 0)
			{
				WriteUnsignedVarInt((uint)transaction.RequestRecords.Count);

				foreach (var record in transaction.RequestRecords)
				{
					Write(record.ContainerId);
					WriteUnsignedVarInt((uint)record.Slots.Count);

					foreach (var slot in record.Slots)
					{
						Write(slot);
					}
				}
			}

			switch (transaction)
			{
				case InventoryMismatchTransaction _:
					WriteUnsignedVarInt((int)McbeInventoryTransaction.TransactionType.InventoryMismatch);
					break;
				case ItemReleaseTransaction _:
					WriteUnsignedVarInt((int)McbeInventoryTransaction.TransactionType.ItemRelease);
					break;
				case ItemUseOnEntityTransaction _:
					WriteUnsignedVarInt((int)McbeInventoryTransaction.TransactionType.ItemUseOnEntity);
					break;
				case ItemUseTransaction _:
					WriteUnsignedVarInt((int)McbeInventoryTransaction.TransactionType.ItemUse);
					break;
				case NormalTransaction _:
					WriteUnsignedVarInt((int)McbeInventoryTransaction.TransactionType.Normal);
					break;
			}


			WriteUnsignedVarInt((uint)transaction.TransactionRecords.Count);
			foreach (var record in transaction.TransactionRecords)
			{
				switch (record)
				{
					case ContainerTransactionRecord r:
						WriteVarInt((int)McbeInventoryTransaction.InventorySourceType.Container);
						WriteSignedVarInt(r.InventoryId);
						break;
					case GlobalTransactionRecord _:
						WriteVarInt((int)McbeInventoryTransaction.InventorySourceType.Global);
						break;
					case WorldInteractionTransactionRecord r:
						WriteVarInt((int)McbeInventoryTransaction.InventorySourceType.WorldInteraction);
						WriteVarInt(r.Flags);
						break;
					case CreativeTransactionRecord _:
						WriteVarInt((int)McbeInventoryTransaction.InventorySourceType.Creative);
						break;
					case CraftTransactionRecord r:
						WriteVarInt((int)McbeInventoryTransaction.InventorySourceType.Crafting);
						WriteVarInt((int)r.Action);
						break;
				}

				WriteVarInt(record.Slot);
				Write(record.OldItem);
				Write(record.NewItem);
			}

			switch (transaction)
			{
				case NormalTransaction _:
				case InventoryMismatchTransaction _:
					break;
				case ItemUseTransaction t:
					WriteUnsignedVarInt((uint)t.ActionType);
					WriteUnsignedVarInt((uint)t.TriggerType);
					Write(t.Position);
					WriteSignedVarInt(t.Face);
					WriteSignedVarInt(t.Slot);
					Write(t.Item);
					Write(t.FromPosition);
					Write(t.ClickPosition);
					WriteUnsignedVarInt(t.BlockRuntimeId);
					Write(t.ClientPredictedResult);
					break;
				case ItemUseOnEntityTransaction t:
					WriteUnsignedVarLong(t.EntityId);
					WriteUnsignedVarInt((uint)t.ActionType);
					WriteSignedVarInt(t.Slot);
					Write(t.Item);
					Write(t.FromPosition);
					Write(t.ClickPosition);
					break;
				case ItemReleaseTransaction t:
					WriteUnsignedVarInt((uint)t.ActionType);
					WriteSignedVarInt(t.Slot);
					Write(t.Item);
					Write(t.FromPosition);
					break;
				default:
					break;
			}
		}

		public Transaction ReadTransaction()
		{
			var requestId = ReadSignedVarInt();
			var requestRecords = new List<RequestRecord>();
			if (requestId != 0)
			{
				var c1 = ReadUnsignedVarInt();
				for (int i = 0; i < c1; i++)
				{
					var rr = new RequestRecord();
					rr.ContainerId = ReadByte();
					var c2 = ReadUnsignedVarInt();
					for (int j = 0; j < c2; j++)
					{
						byte slot = ReadByte();
						rr.Slots.Add(slot);
					}

					requestRecords.Add(rr);
				}
			}

			var transactionType = (McbeInventoryTransaction.TransactionType)ReadVarInt();


			var transactions = new List<TransactionRecord>();
			uint count = ReadUnsignedVarInt();
			for (int i = 0; i < count; i++)
			{
				TransactionRecord record;
				int sourceType = ReadVarInt();
				switch ((McbeInventoryTransaction.InventorySourceType)sourceType)
				{
					case McbeInventoryTransaction.InventorySourceType.Container:
						record = new ContainerTransactionRecord() { InventoryId = ReadSignedVarInt() };
						break;
					case McbeInventoryTransaction.InventorySourceType.Global:
						record = new GlobalTransactionRecord();
						break;
					case McbeInventoryTransaction.InventorySourceType.WorldInteraction:
						record = new WorldInteractionTransactionRecord() { Flags = ReadVarInt() };
						break;
					case McbeInventoryTransaction.InventorySourceType.Creative:
						record = new CreativeTransactionRecord() { InventoryId = 0x79 };
						break;
					case McbeInventoryTransaction.InventorySourceType.Unspecified:
					case McbeInventoryTransaction.InventorySourceType.Crafting:
						record = new CraftTransactionRecord()
							{ Action = (McbeInventoryTransaction.CraftingAction)ReadSignedVarInt() };
						break;
					default:
						Console.WriteLine($"Unknown inventory source type={sourceType}");
						continue;
				}

				record.Slot = ReadVarInt();
				record.OldItem = ReadItem();
				record.NewItem = ReadItem();


				transactions.Add(record);
			}

			Transaction transaction = null;
			switch (transactionType)
			{
				case McbeInventoryTransaction.TransactionType.Normal:
					transaction = new NormalTransaction();
					break;
				case McbeInventoryTransaction.TransactionType.InventoryMismatch:
					transaction = new InventoryMismatchTransaction();
					break;
				case McbeInventoryTransaction.TransactionType.ItemUse:
					transaction = new ItemUseTransaction()
					{
						ActionType = (McbeInventoryTransaction.ItemUseAction)ReadVarInt(),
						TriggerType = (McbeInventoryTransaction.TriggerType)ReadVarInt(),
						Position = ReadBlockCoordinates(),
						Face = ReadSignedVarInt(),
						Slot = ReadSignedVarInt(),
						Item = ReadItem(),
						FromPosition = ReadVector3(),
						ClickPosition = ReadVector3(),
						BlockRuntimeId = ReadUnsignedVarInt(),
						ClientPredictedResult = ReadUnsignedVarInt()
					};
					break;
				case McbeInventoryTransaction.TransactionType.ItemUseOnEntity:
					transaction = new ItemUseOnEntityTransaction()
					{
						EntityId = ReadVarLong(),
						ActionType = (McbeInventoryTransaction.ItemUseOnEntityAction)ReadVarInt(),
						Slot = ReadSignedVarInt(),
						Item = ReadItem(),
						FromPosition = ReadVector3(),
						ClickPosition = ReadVector3()
					};
					break;
				case McbeInventoryTransaction.TransactionType.ItemRelease:
					transaction = new ItemReleaseTransaction()
					{
						ActionType = (McbeInventoryTransaction.ItemReleaseAction)ReadVarInt(),
						Slot = ReadSignedVarInt(),
						Item = ReadItem(),
						FromPosition = ReadVector3()
					};
					break;
			}

			transaction.TransactionRecords = transactions;
			transaction.RequestId = requestId;
			transaction.RequestRecords = requestRecords;

			return transaction;
		}

		public StackRequestSlotInfo ReadStackRequestSlotInfo()
		{
			var containerName = readFullContainerName();
			var slot = (byte)ReadByte();
			var stackNetworkId = ReadSignedVarInt();


			return new StackRequestSlotInfo()
			{
				ContainerId = containerName.ContainerID,
				Slot = slot,
				StackNetworkId = stackNetworkId,
				DynamicId = (int)containerName.DynamicContainerID.Value
			};
		}

		public FullContainerName readFullContainerName()
		{
			var name = new FullContainerName();
			name.DynamicContainerID = new Optional<uint>();
			name.ContainerID = ReadByte();
			var readBool = ReadBool();
			if (readBool)
			{
				name.DynamicContainerID.HasValue = true;
				name.DynamicContainerID.Value = ReadUint();
			}
			else
			{
				name.DynamicContainerID.HasValue = readBool;
			}

			return name;
		}

		public void Write(FullContainerName name)
		{
			Write(name.ContainerID);
			Write(name.DynamicContainerID.HasValue);
			if (name.DynamicContainerID.HasValue)
			{
				Write(name.DynamicContainerID.Value);
			}
		}

		public void Write(StackRequestSlotInfo slotInfo)
		{
			Write(new FullContainerName()
			{
				ContainerID = slotInfo.ContainerId, DynamicContainerID = new Optional<uint>((uint)slotInfo.DynamicId)
			});
			Write(slotInfo.Slot);
			WriteSignedVarInt(slotInfo.StackNetworkId);
		}

		public void Write(ItemStackRequests requests)
		{
			WriteUnsignedVarInt((uint)requests.Count);

			foreach (ItemStackActionList request in requests)
			{
				WriteSignedVarInt(request.RequestId);
				WriteUnsignedVarInt((uint)request.Count);

				foreach (ItemStackAction action in request)
				{
					switch (action)
					{
						case TakeAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.Take);
							Write(ta.Count);
							Write(ta.Source);
							Write(ta.Destination);
							break;
						}

						case PlaceAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.Place);
							Write(ta.Count);
							Write(ta.Source);
							Write(ta.Destination);
							break;
						}

						case SwapAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.Swap);
							Write(ta.Source);
							Write(ta.Destination);
							break;
						}

						case DropAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.Drop);
							Write(ta.Count);
							Write(ta.Source);
							Write(ta.Randomly);
							break;
						}

						case DestroyAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.Destroy);
							Write(ta.Count);
							Write(ta.Source);
							break;
						}

						case ConsumeAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.Consume);
							Write(ta.Count);
							Write(ta.Source);
							break;
						}

						case CreateAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.Create);
							Write(ta.ResultSlot);
							break;
						}

						case PlaceIntoBundleAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.PlaceIntoBundleDeprecated);
							break;
						}

						case TakeFromBundleAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.TakeFromBundleDeprecated);
							break;
						}

						case LabTableCombineAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.LabTableCombine);
							break;
						}

						case BeaconPaymentAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.BeaconPayment);
							WriteSignedVarInt(ta.PrimaryEffect);
							WriteSignedVarInt(ta.SecondaryEffect);
							break;
						}

						case CraftAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.CraftRecipe);
							WriteUnsignedVarInt(ta.RecipeNetworkId);
							Write(ta.TimesCrafted);
							break;
						}

						case CraftAutoAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.CraftRecipeAuto);
							WriteUnsignedVarInt(ta.RecipeNetworkId);
							Write(ta.TimesCrafted2);
							Write(ta.TimesCrafted);
							Write((byte)ta.Ingredients.Count);
							foreach (Item item in ta.Ingredients)
							{
								WriteRecipeIngredient(item);
							}

							break;
						}

						case CraftCreativeAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.CraftCreative);
							WriteUnsignedVarInt(ta.CreativeItemNetworkId);
							Write(ta.ClientPredictedResult);
							break;
						}

						case CraftRecipeOptionalAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.CraftRecipeOptional);
							WriteUnsignedVarInt(ta.RecipeNetworkId);
							Write(ta.FilteredStringIndex);
							break;
						}

						case GrindstoneStackRequestAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.CraftGrindstone);
							WriteUnsignedVarInt(ta.RecipeNetworkId);
							WriteVarInt(ta.RepairCost);
							Write(ta.TimesCrafted);
							break;
						}

						case LoomStackRequestAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.CraftLoom);
							Write(ta.PatternId);
							Write(ta.TimesCrafted);
							break;
						}

						case CraftNotImplementedDeprecatedAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.CraftNotImplementedDeprecated);
							break;
						}

						case CraftResultDeprecatedAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.CraftResultsDeprecated);
							Write(ta.ResultItems);
							Write(ta.TimesCrafted);
							break;
						}

						case MineBlockAction ta:
						{
							Write((byte)McbeItemStackRequest.ActionType.MineBlock);
							WriteVarInt(ta.Slot);
							WriteVarInt(ta.Durability);
							WriteSignedVarInt(ta.stackNetworkId);
							break;
						}
					}
				}

				WriteUnsignedVarInt((uint)request.filteredString.Count);

				for (int fi = 0; fi < request.filteredString.Count; fi++)
				{
					Write(request.filteredString[fi]);
				}

				Write(request.FilterCause);
			}
		}

		public ItemStackRequests ReadItemStackRequests(bool single = false)
		{
			var requests = new ItemStackRequests();

			uint c = 1;

			if (!single)
			{
				c = ReadUnsignedVarInt();
			}


			for (int i = 0; i < c; i++)
			{
				var actions = new ItemStackActionList();
				actions.RequestId = ReadSignedVarInt();


				uint count = ReadUnsignedVarInt();

				for (int j = 0; j < count; j++)
				{
					var actionType = (McbeItemStackRequest.ActionType)ReadByte();

					switch (actionType)
					{
						case McbeItemStackRequest.ActionType.Take:
						{
							var action = new TakeAction();
							action.Count = ReadByte();
							action.Source = ReadStackRequestSlotInfo();
							action.Destination = ReadStackRequestSlotInfo();
							actions.Add(action);
							break;
						}
						case McbeItemStackRequest.ActionType.Place:
						{
							var action = new PlaceAction();
							action.Count = ReadByte();
							action.Source = ReadStackRequestSlotInfo();
							action.Destination = ReadStackRequestSlotInfo();
							actions.Add(action);
							break;
						}
						case McbeItemStackRequest.ActionType.Swap:
						{
							var action = new SwapAction();
							action.Source = ReadStackRequestSlotInfo();
							action.Destination = ReadStackRequestSlotInfo();
							actions.Add(action);
							break;
						}
						case McbeItemStackRequest.ActionType.Drop:
						{
							var action = new DropAction();
							action.Count = ReadByte();
							action.Source = ReadStackRequestSlotInfo();
							action.Randomly = ReadBool();
							actions.Add(action);
							break;
						}
						case McbeItemStackRequest.ActionType.Destroy:
						{
							var action = new DestroyAction();
							action.Count = ReadByte();
							action.Source = ReadStackRequestSlotInfo();
							actions.Add(action);
							break;
						}
						case McbeItemStackRequest.ActionType.Consume:
						{
							var action = new ConsumeAction();
							action.Count = ReadByte();
							action.Source = ReadStackRequestSlotInfo();
							actions.Add(action);
							break;
						}
						case McbeItemStackRequest.ActionType.Create:
						{
							var action = new CreateAction();
							action.ResultSlot = ReadByte();
							actions.Add(action);
							break;
						}

						case McbeItemStackRequest.ActionType.PlaceIntoBundleDeprecated:
						{
							var action = new PlaceIntoBundleAction();
							actions.Add(action);
							break;
						}

						case McbeItemStackRequest.ActionType.TakeFromBundleDeprecated:
						{
							var action = new TakeFromBundleAction();
							actions.Add(action);
							break;
						}
						case McbeItemStackRequest.ActionType.LabTableCombine:
						{
							var action = new LabTableCombineAction();
							actions.Add(action);
							break;
						}
						case McbeItemStackRequest.ActionType.BeaconPayment:
						{
							var action = new BeaconPaymentAction();
							action.PrimaryEffect = ReadSignedVarInt();
							action.SecondaryEffect = ReadSignedVarInt();
							actions.Add(action);
							break;
						}
						case McbeItemStackRequest.ActionType.CraftRecipe:
						{
							var action = new CraftAction();
							action.RecipeNetworkId = ReadUnsignedVarInt();
							action.TimesCrafted = ReadByte();
							actions.Add(action);
							break;
						}
						case McbeItemStackRequest.ActionType.CraftRecipeAuto:
						{
							var action = new CraftAutoAction();
							action.RecipeNetworkId = ReadUnsignedVarInt();
							action.TimesCrafted2 = ReadByte();
							action.TimesCrafted = ReadByte();
							var cou = ReadByte();
							for (var a = 0; a < cou; a++)
							{
								action.Ingredients.Add(ReadRecipeData());
							}

							actions.Add(action);
							break;
						}
						case McbeItemStackRequest.ActionType.CraftCreative:
						{
							var action = new CraftCreativeAction();
							action.CreativeItemNetworkId = ReadUnsignedVarInt();
							action.ClientPredictedResult = ReadByte();
							actions.Add(action);
							break;
						}
						case McbeItemStackRequest.ActionType.CraftRecipeOptional:
						{
							var action = new CraftRecipeOptionalAction();
							action.RecipeNetworkId = ReadUnsignedVarInt();
							action.FilteredStringIndex = ReadInt();
							actions.Add(action);
							break;
						}
						case McbeItemStackRequest.ActionType.CraftGrindstone:
						{
							var action = new GrindstoneStackRequestAction();
							action.RecipeNetworkId = ReadUnsignedVarInt();
							action.RepairCost = ReadVarInt();
							action.TimesCrafted = ReadByte();
							actions.Add(action);
							break;
						}
						case McbeItemStackRequest.ActionType.CraftLoom:
						{
							var action = new LoomStackRequestAction();
							action.PatternId = ReadString();
							action.TimesCrafted = ReadByte();
							actions.Add(action);
							break;
						}
						case McbeItemStackRequest.ActionType.CraftNotImplementedDeprecated:
						{
							var action = new CraftNotImplementedDeprecatedAction();
							actions.Add(action);
							break;
						}
						case McbeItemStackRequest.ActionType.CraftResultsDeprecated:
						{
							var action = new CraftResultDeprecatedAction();
							action.ResultItems = ReadItems();
							action.TimesCrafted = ReadByte();
							actions.Add(action);
							break;
						}
						case McbeItemStackRequest.ActionType.MineBlock:
						{
							var action = new MineBlockAction();
							action.Slot = ReadVarInt();
							action.Durability = ReadVarInt();
							action.stackNetworkId = ReadSignedVarInt();
							actions.Add(action);
							break;
						}
						default:
							throw new ArgumentOutOfRangeException();
					}
				}

				requests.Add(actions);

				var filterStringCount = ReadUnsignedVarInt();

				for (int fi = 0; fi < filterStringCount; fi++)
				{
					actions.filteredString.Add(ReadString());
				}

				var filterStringCause = ReadUint();
			}

			return requests;
		}

		public void Write(ItemStackResponses responses)
		{
			WriteUnsignedVarInt((uint)responses.Count);
			foreach (ItemStackResponse stackResponse in responses)
			{
				Write((byte)stackResponse.Result);
				WriteSignedVarInt(stackResponse.RequestId);
				if (stackResponse.Result != StackResponseStatus.Ok)
					continue;
				WriteUnsignedVarInt((uint)stackResponse.ResponseContainerInfos.Count);
				foreach (StackResponseContainerInfo containerInfo in stackResponse.ResponseContainerInfos)
				{
					Write(new FullContainerName()
					{
						ContainerID = containerInfo.ContainerId,
						DynamicContainerID = new Optional<uint>((uint)containerInfo.DynamicId)
					});
					WriteUnsignedVarInt((uint)containerInfo.Slots.Count);
					foreach (StackResponseSlotInfo slot in containerInfo.Slots)
					{
						Write(slot.Slot);
						Write(slot.HotbarSlot);
						Write(slot.Count);
						WriteSignedVarInt(slot.StackNetworkId);
						Write(slot.CustomName);
						Write(slot.FilteredCustomName);
						WriteSignedVarInt(slot.DurabilityCorrection);
					}
				}
			}
		}


		public ItemStackResponses ReadItemStackResponses()
		{
			var responses = new ItemStackResponses();
			var count = ReadUnsignedVarInt();

			for (var i = 0; i < count; i++)
			{
				var response = new ItemStackResponse();
				response.Result = (StackResponseStatus)ReadByte();
				response.RequestId = ReadSignedVarInt();

				if (response.Result != StackResponseStatus.Ok)
					continue;

				response.ResponseContainerInfos = new List<StackResponseContainerInfo>();
				var subCount = ReadUnsignedVarInt();
				for (int sub = 0; sub < subCount; sub++)
				{
					var containerInfo = new StackResponseContainerInfo();
					var name = readFullContainerName();
					containerInfo.ContainerId = name.ContainerID;
					containerInfo.DynamicId = (int)name.DynamicContainerID.Value;
					var slotCount = ReadUnsignedVarInt();
					containerInfo.Slots = new List<StackResponseSlotInfo>();

					for (int si = 0; si < slotCount; si++)
					{
						var slot = new StackResponseSlotInfo();
						slot.Slot = ReadByte();
						slot.HotbarSlot = ReadByte();
						slot.Count = ReadByte();
						slot.StackNetworkId = ReadSignedVarInt();
						slot.CustomName = ReadString();
						slot.FilteredCustomName = ReadString();
						slot.DurabilityCorrection = ReadSignedVarInt();

						containerInfo.Slots.Add(slot);
					}

					response.ResponseContainerInfos.Add(containerInfo);
				}

				responses.Add(response);
			}

			return responses;
		}

		public void Write(EnchantOptions options)
		{
			WriteUnsignedVarInt((uint)options.Count);
			foreach (EnchantOption option in options)
			{
				WriteUnsignedVarInt(option.Cost);
				Write(option.Flags);
				WriteEnchants(option.EquipActivatedEnchantments);
				WriteEnchants(option.HeldActivatedEnchantments);
				WriteEnchants(option.SelfActivatedEnchantments);
				Write(option.Name);
				WriteVarInt(option.OptionId);
			}
		}

		private void WriteEnchants(List<Enchant> enchants)
		{
			WriteUnsignedVarInt((uint)enchants.Count);
			foreach (Enchant enchant in enchants)
			{
				Write(enchant.Id);
				Write(enchant.Level);
			}
		}

		private List<Enchant> ReadEnchants()
		{
			List<Enchant> enchants = new List<Enchant>();
			var count = ReadUnsignedVarInt();

			for (int i = 0; i < count; i++)
			{
				Enchant enchant = new Enchant(ReadByte(), ReadByte());
				enchants.Add(enchant);
			}

			return enchants;
		}

		public EnchantOptions ReadEnchantOptions()
		{
			var options = new EnchantOptions();
			var count = ReadUnsignedVarInt();

			for (int i = 0; i < count; i++)
			{
				EnchantOption option = new EnchantOption();
				option.Cost = ReadUnsignedVarInt();
				option.Flags = ReadInt();
				option.EquipActivatedEnchantments = ReadEnchants();
				option.HeldActivatedEnchantments = ReadEnchants();
				option.SelfActivatedEnchantments = ReadEnchants();
				option.Name = ReadString();
				option.OptionId = ReadVarInt();

				options.Add(option);
			}

			return options;
		}

		public void Write(AnimationKey[] keys)
		{
			WriteUnsignedVarInt((uint)keys.Length);
			foreach (AnimationKey key in keys)
			{
				Write(key.ExecuteImmediate);
				Write(key.ResetBefore);
				Write(key.ResetAfter);
				Write(key.StartRotation);
				Write(key.EndRotation);
				WriteUnsignedVarInt(key.Duration);
			}
		}

		public AnimationKey[] ReadAnimationKeys()
		{
			var count = ReadUnsignedVarInt();
			var keys = new AnimationKey[count];
			for (int i = 0; i < count; i++)
			{
				AnimationKey key = new AnimationKey();
				key.ExecuteImmediate = ReadBool();
				key.ResetBefore = ReadBool();
				key.ResetAfter = ReadBool();
				key.StartRotation = ReadVector3();
				key.EndRotation = ReadVector3();
				key.Duration = ReadUnsignedVarInt();
				keys[i] = key;
			}

			return keys;
		}


		private ItemStacks ReadItems()
		{
			var items = new ItemStacks();

			var count = ReadUnsignedVarInt();

			for (int i = 0; i < count; i++)
			{
				items.Add(ReadItem(false));
			}

			return items;
		}

		private const int ShieldId = 355;

		public void Write(Item stack, bool writeUniqueId = true)
		{
			stack = new ItemAir();
			var netData = new TranslatedItem(0, 0);
			if (stack == null || stack.Id == 0)
			{
				WriteSignedVarInt(0);
				return;
			}

			WriteSignedVarInt(netData.Id);
			Write((short)stack.Count);
			WriteUnsignedVarInt((uint)netData.Meta);

			if (writeUniqueId)
			{
				Write(stack.UniqueId != 0);

				if (stack.UniqueId != 0)
				{
					WriteVarInt(stack.UniqueId);
				}
			}

			WriteSignedVarInt(stack.RuntimeId);

			byte[] extraData = null;

			using (var ms = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(ms, Encoding.UTF8, true))
				{
					if (stack.ExtraData != null)
					{
						binaryWriter.Write((ushort)0xffff);
						binaryWriter.Write((byte)1);
						var nbtData = GetNbtData(stack.ExtraData, false);
						binaryWriter.Write(nbtData);
					}
					else
					{
						binaryWriter.Write((short)0);
					}

					binaryWriter.Write(0);
					binaryWriter.Write(0);

					if (stack.Id == 513)
					{
						binaryWriter.Write((long)0);
					}
				}

				extraData = ms.ToArray();
			}

			WriteLength(extraData.Length);
			Write(extraData);
		}

		public Item ReadItem(bool readUniqueId = true)
		{
			int id = ReadSignedVarInt();
			if (id == 0)
			{
				return new ItemAir();
			}

			short count = (short)ReadShort();
			var metadata = ReadUnsignedVarInt();


			Item stack = new ItemAir();

			if (readUniqueId)
			{
				if (ReadBool()) stack.UniqueId = ReadVarInt();
			}

			stack.RuntimeId = ReadSignedVarInt();

			int length = ReadLength();
			var data = ReadBytes(length);

			using (MemoryStream ms = new MemoryStream(data))
			{
				using (BinaryReader binaryReader = new BinaryReader(ms))
				{
					ushort nbtLen = binaryReader.ReadUInt16();
					if (nbtLen == 0xffff)
					{
						byte version = binaryReader.ReadByte();

						if (version != 1)
						{
							throw new Exception($"Fringe nbt version when reading item extra NBT: {version}");
						}

						var beforeRead = ms.Position;
						stack.ExtraData = ReadNbtCompound(ms, false);
						var afterRead = ms.Position;
						var nbtCompoundLength = afterRead - beforeRead;
					}
					else if (nbtLen > 0)
					{
						throw new Exception($"Fringe nbt length when reading item extra NBT: {nbtLen}");
					}

					int canPlace = binaryReader.ReadInt32();
					for (int i = 0; i < canPlace; i++)
					{
						var l = binaryReader.ReadInt16();
						binaryReader.ReadBytes(l);
					}

					int canBreak = binaryReader.ReadInt32();
					for (int i = 0; i < canBreak; i++)
					{
						var l = binaryReader.ReadInt16();
						binaryReader.ReadBytes(l);
					}

					if (stack.RuntimeId == ShieldId)
					{
						binaryReader.ReadInt64();
					}
				}
			}

			return stack;
		}


		public static byte[] GetNbtData(NbtCompound nbtCompound, bool useVarInt = true)
		{
			nbtCompound.Name = string.Empty;
			var file = new NbtFile(nbtCompound);
			file.BigEndian = false;
			file.UseVarInt = useVarInt;

			return file.SaveToBuffer(NbtCompression.None);
		}

		public void Write(MetadataDictionary metadata)
		{
			if (metadata != null)
			{
				metadata.WriteTo(_writer);
			}
		}

		public MetadataDictionary ReadMetadataDictionary()
		{
			var reader = new BinaryReader(_reader);
			var dictionary = MetadataDictionary.FromStream(reader);

			return dictionary;
		}

		public AttributeModifiers ReadAttributeModifiers()
		{
			var modifiers = new AttributeModifiers();
			uint count = ReadUnsignedVarInt();
			for (int i = 0; i < count; i++)
			{
				AttributeModifier modifier = new AttributeModifier
				{
					Id = ReadString(),
					Name = ReadString(),
					Amount = ReadFloat(),
					Operations = ReadInt(),
					Operand = ReadInt(),
					Serializable = ReadBool(),
				};
				modifiers[modifier.Name] = modifier;
			}

			return modifiers;
		}

		public void Write(AttributeModifiers modifiers)
		{
			WriteUnsignedVarInt((uint)modifiers.Count);
			foreach (AttributeModifier modifier in modifiers.Values)
			{
				Write(modifier.Id);
				Write(modifier.Name);
				Write(modifier.Amount);
				Write(modifier.Operations);
				Write(modifier.Operand);
				Write(modifier.Serializable);
			}
		}

		public PlayerAttributes ReadPlayerAttributes()
		{
			var attributes = new PlayerAttributes();
			uint count = ReadUnsignedVarInt();
			for (int i = 0; i < count; i++)
			{
				PlayerAttribute attribute = new PlayerAttribute
				{
					MinValue = ReadFloat(),
					MaxValue = ReadFloat(),
					Value = ReadFloat(),
					DefaultMinValue = ReadFloat(),
					DefaultMaxValue = ReadFloat(),
					Default = ReadFloat(),
					Name = ReadString(),
					Modifiers = ReadAttributeModifiers()
				};
				attributes[attribute.Name] = attribute;
			}

			return attributes;
		}

		public void Write(PlayerAttributes attributes)
		{
			WriteUnsignedVarInt((uint)attributes.Count);
			foreach (PlayerAttribute attribute in attributes.Values)
			{
				Write(attribute.MinValue);
				Write(attribute.MaxValue);
				Write(attribute.Value);
				Write(attribute.DefaultMinValue == 0.0f ? attribute.MinValue : attribute.DefaultMinValue);
				Write(attribute.DefaultMaxValue == 0.0f ? attribute.MaxValue : attribute.DefaultMaxValue);
				Write(attribute.Default);
				Write(attribute.Name);
				Write(attribute.Modifiers);
			}
		}


		public GameRules ReadGameRules()
		{
			GameRules gameRules = new GameRules();

			int count = ReadVarInt();
			for (int i = 0; i < count; i++)
			{
				string name = ReadString();
				bool isPlayerModifiable = ReadBool();
				var type = ReadUnsignedVarInt();
				switch (type)
				{
					case 1:
					{
						GameRule<bool> rule = new GameRule<bool>(name, ReadBool())
						{
							IsPlayerModifiable = isPlayerModifiable
						};
						gameRules.Add(rule);
						break;
					}
					case 2:
					{
						GameRule<int> rule = new GameRule<int>(name, ReadVarInt())
						{
							IsPlayerModifiable = isPlayerModifiable
						};
						gameRules.Add(rule);
						break;
					}
					case 3:
					{
						GameRule<float> rule = new GameRule<float>(name, ReadFloat())
						{
							IsPlayerModifiable = isPlayerModifiable
						};
						gameRules.Add(rule);
						break;
					}
				}
			}

			return gameRules;
		}

		public void Write(GameRules gameRules)
		{
			if (gameRules == null)
			{
				WriteVarInt(0);
				return;
			}

			WriteVarInt(gameRules.Count);
			foreach (var rule in gameRules)
			{
				Write(rule.Name.ToLower());
				Write(rule.IsPlayerModifiable);

				if (rule is GameRule<bool>)
				{
					WriteUnsignedVarInt(1);
					Write(((GameRule<bool>)rule).Value);
				}
				else if (rule is GameRule<int>)
				{
					WriteUnsignedVarInt(2);
					WriteVarInt(((GameRule<int>)rule).Value);
				}
				else if (rule is GameRule<float>)
				{
					WriteUnsignedVarInt(3);
					Write(((GameRule<float>)rule).Value);
				}
			}
		}

		public void Write(EntityAttributes attributes)
		{
			if (attributes == null)
			{
				WriteUnsignedVarInt(0);
				return;
			}

			WriteUnsignedVarInt((uint)attributes.Count);
			foreach (EntityAttribute attribute in attributes.Values)
			{
				Write(attribute.Name);
				Write(attribute.MinValue);
				Write(attribute.Value);
				Write(attribute.MaxValue);
			}
		}

		public EntityAttributes ReadEntityAttributes()
		{
			var attributes = new EntityAttributes();
			uint count = ReadUnsignedVarInt();
			for (int i = 0; i < count; i++)
			{
				EntityAttribute attribute = new EntityAttribute
				{
					Name = ReadString(),
					MinValue = ReadFloat(),
					Value = ReadFloat(),
					MaxValue = ReadFloat(),
				};

				attributes[attribute.Name] = attribute;
			}

			return attributes;
		}

		public Itemstates ReadItemstates()
		{
			var result = new Itemstates();
			uint count = ReadUnsignedVarInt();
			for (int runtimeId = 0; runtimeId < count; runtimeId++)
			{
				var name = ReadString();
				var legacyId = ReadShort();
				var component = ReadBool();
				var version = ReadVarInt();
				var components = ReadNbt();

				byte[] componentValue = new byte[0];

				if (components.NbtFile.RootTag["components"] != null)
				{
					using (MemoryStream stream = new MemoryStream())
					{
						NbtFile file = new NbtFile(components.NbtFile.RootTag["components"] as NbtCompound);
						file.SaveToStream(stream, NbtCompression.None);
						componentValue = stream.ToArray();
					}
				}

				result.Add(new Itemstate
				{
					Id = legacyId,
					Name = name,
					ComponentBased = component,
					Version = version,
					Components = componentValue
				});
			}

			return result;
		}

		public void Write(ParameterKeyframeValue value)
		{
			Write(value.Time);
			Write(value.Value);
		}

		public ParameterKeyframeValue ReadParameterKeyframeValue()
		{
			return new ParameterKeyframeValue()
			{
				Time = ReadFloat(),
				Value = ReadVector3()
			};
		}

		public void Write(Itemstates itemstates)
		{
			if (itemstates == null)
			{
				WriteUnsignedVarInt(0);
				return;
			}

			WriteUnsignedVarInt((uint)itemstates.Count);
			foreach (var itemstate in itemstates)
			{
				Write(itemstate.Name);
				Write(itemstate.Id);
				Write(itemstate.ComponentBased);
				WriteVarInt(itemstate.Version);
				Nbt nbt = new Nbt
				{
					NbtFile = new NbtFile
					{
						BigEndian = false,
						UseVarInt = true,
						RootTag = new NbtCompound("")
					}
				};
				if (itemstate.Components.Count() > 0)
				{
					using (MemoryStream stream = new MemoryStream(itemstate.Components))
					{
						NbtFile file = new NbtFile();
						file.LoadFromStream(stream, NbtCompression.None);
						var componentNbt = new NbtCompound("")
						{
							file.RootTag as NbtCompound
						};
						nbt.NbtFile.RootTag = componentNbt;
					}
				}

				Write(nbt);
			}
		}

		public BlockPalette ReadBlockPalette()
		{
			var result = new BlockPalette();
			var count = ReadUnsignedVarInt();

			for (int runtimeId = 0; runtimeId < count; runtimeId++)
			{
				var record = new BlockStateContainer();
				record.Id = record.RuntimeId = runtimeId;
				record.Name = ReadString();
				record.States = new List<IBlockState>();

				var nbt = ReadNbt(_reader);
				var rootTag = nbt.NbtFile.RootTag;

				foreach (var state in GetBlockStates(rootTag))
				{
					record.States.Add(state);
				}
			}

			return result;
		}

		private IEnumerable<IBlockState> GetBlockStates(NbtTag tag)
		{
			switch (tag.TagType)
			{
				case NbtTagType.List:
				{
					foreach (var state in GetBlockStatesFromList((NbtList)tag))
						yield return state;
				}
					break;

				case NbtTagType.Compound:
				{
					foreach (var state in GetBlockStatesFromCompound((NbtCompound)tag))
						yield return state;
				}
					break;

				default:
				{
					if (TryGetStateFromTag(tag, out var state))
						yield return state;
				}
					break;
			}
		}

		private IEnumerable<IBlockState> GetBlockStatesFromCompound(NbtCompound list)
		{
			if (list.TryGet("states", out NbtTag states))
			{
				foreach (var state in GetBlockStates(states))
				{
					yield return state;
				}
			}
		}


		private IEnumerable<IBlockState> GetBlockStatesFromList(NbtList list)
		{
			foreach (NbtTag tag in list)
			{
				if (TryGetStateFromTag(tag, out var state))
				{
					yield return state;
				}
				else
				{
					foreach (var s in GetBlockStates(tag))
					{
						yield return s;
					}
				}
			}
		}

		private bool TryGetStateFromTag(NbtTag tag, out IBlockState state)
		{
			switch (tag.TagType)
			{
				case NbtTagType.Byte:
					state = new BlockStateByte()
					{
						Name = tag.Name,
						Value = tag.ByteValue
					};
					return true;

				case NbtTagType.Int:
					state = new BlockStateInt()
					{
						Name = tag.Name,
						Value = tag.IntValue
					};
					return true;

				case NbtTagType.String:
					state = new BlockStateString()
					{
						Name = tag.Name,
						Value = tag.StringValue
					};
					return true;
			}

			state = null;

			return false;
		}

		public void Write(BlockPalette palette)
		{
			if (palette == null)
			{
				WriteUnsignedVarInt(0);
				return;
			}

			WriteUnsignedVarInt((uint)palette.Count);
			foreach (BlockStateContainer record in palette.Values)
			{
				Write(record.Name);
				Write(record.StatesCacheNbt);
			}
		}

		public void Write(AbilityLayer layer)
		{
			Write((ushort)layer.Type);
			Write((uint)layer.Abilities);
			Write((uint)layer.Values);
			Write(layer.FlySpeed);
			Write(layer.WalkSpeed);
			Write(layer.VerticalFlySpeed);
		}

		public AbilityLayer ReadAbilityLayer()
		{
			AbilityLayer layer = new AbilityLayer();
			layer.Type = (AbilityLayerType)ReadUshort();
			layer.Abilities = (PlayerAbility)ReadUint();
			layer.Values = ReadUint();
			layer.FlySpeed = ReadFloat();
			layer.WalkSpeed = ReadFloat();
			layer.VerticalFlySpeed = ReadFloat();

			return layer;
		}

		public void Write(AbilityLayers layers)
		{
			Write((byte)layers.Count);

			foreach (var layer in layers)
			{
				Write(layer);
			}
		}

		public AbilityLayers ReadAbilityLayers()
		{
			AbilityLayers layers = new AbilityLayers();
			var count = ReadByte();

			for (int i = 0; i < count; i++)
			{
				layers.Add(ReadAbilityLayer());
			}

			return layers;
		}

		public void Write(EntityLink link)
		{
			WriteVarLong(link.FromEntityId);
			WriteVarLong(link.ToEntityId);
			Write((byte)link.Type);
			Write(link.Immediate);
			Write(link.CausedByRider);
			Write(link.VehicleAngularVelocity);
		}

		public EntityLink ReadEntityLink()
		{
			var from = ReadVarLong();
			var to = ReadVarLong();
			var type = (EntityLink.EntityLinkType)ReadByte();
			var immediate = ReadBool();
			var causedByRider = ReadBool();
			var vehicleAngularVelocity = ReadFloat();

			return new EntityLink(from, to, type, immediate, causedByRider, vehicleAngularVelocity);
		}

		public void Write(EntityLinks links)
		{
			if (links == null)
			{
				WriteUnsignedVarInt(0);
				return;
			}

			WriteUnsignedVarInt((uint)links.Count);
			foreach (var link in links)
			{
				Write(link);
			}
		}

		public EntityLinks ReadEntityLinks()
		{
			var count = ReadUnsignedVarInt();

			var links = new EntityLinks();
			for (int i = 0; i < count; i++)
			{
				links.Add(ReadEntityLink());
			}

			return links;
		}

		public void Write(Rules rules)
		{
			_writer.Write(rules.Count);
			foreach (var rule in rules)
			{
				Write(rule.Name);
				Write(rule.Unknown1);
				Write(rule.Unknown2);
			}
		}

		public Rules ReadRules()
		{
			int count = _reader.ReadInt32();

			var rules = new Rules();
			for (int i = 0; i < count; i++)
			{
				RuleData rule = new RuleData();
				rule.Name = ReadString();
				rule.Unknown1 = ReadBool();
				rule.Unknown2 = ReadBool();
				rules.Add(rule);
			}

			return rules;
		}

		public void Write(TexturePackInfos packInfos)
		{
			if (packInfos == null)
			{
				_writer.Write((short)0);

				return;
			}

			_writer.Write((short)packInfos.Count);

			foreach (var info in packInfos)
			{
				Write(info.UUID);
				Write(info.Version);
				Write(info.Size);
				Write(info.ContentKey);
				Write(info.SubPackName);
				Write(info.ContentIdentity);
				Write(info.HasScripts);
				Write(info.isAddon);
				Write(info.RtxEnabled);
				Write(info.cndUrls);
			}
		}

		public TexturePackInfos ReadTexturePackInfos()
		{
			int count = _reader.ReadInt16();


			var packInfos = new TexturePackInfos();
			for (int i = 0; i < count; i++)
			{
				var info = new TexturePackInfo();
				var id = ReadUUID();
				var version = ReadString();
				var size = ReadUlong();
				var encryptionKey = ReadString();
				var subpackName = ReadString();
				var contentIdentity = ReadString();
				var hasScripts = ReadBool();
				var isAddon = ReadBool();
				var rtxEnabled = ReadBool();
				var cndUrls = ReadString();


				info.UUID = id;
				info.Version = version;
				info.Size = size;
				info.HasScripts = hasScripts;
				info.ContentKey = encryptionKey;
				info.SubPackName = subpackName;
				info.ContentIdentity = contentIdentity;
				info.isAddon = isAddon;
				info.RtxEnabled = rtxEnabled;
				info.cndUrls = cndUrls;

				packInfos.Add(info);
			}

			return packInfos;
		}

		public void Write(ResourcePackInfos packInfos)
		{
			if (packInfos == null)
			{
				_writer.Write((short)0);
				return;
			}

			_writer.Write((short)packInfos.Count);

			foreach (var info in packInfos)
			{
				Write(info.UUID);
				Write(info.Version);
				Write(info.Size);
				Write(info.ContentKey);
				Write(info.SubPackName);
				Write(info.ContentIdentity);
				Write(info.HasScripts);
				Write(info.isAddon);
			}
		}

		public ResourcePackInfos ReadResourcePackInfos()
		{
			int count = _reader.ReadInt16();


			var packInfos = new ResourcePackInfos();
			for (int i = 0; i < count; i++)
			{
				var info = new ResourcePackInfo();

				var id = ReadUUID();
				var version = ReadString();
				var size = ReadUlong();
				var encryptionKey = ReadString();
				var subpackName = ReadString();
				var contentIdentity = ReadString();
				var hasScripts = ReadBool();
				var isAddon = ReadBool();

				info.UUID = id;
				info.Version = version;
				info.Size = size;
				info.ContentKey = encryptionKey;
				info.SubPackName = subpackName;
				info.ContentIdentity = contentIdentity;
				info.HasScripts = hasScripts;
				info.isAddon = isAddon;

				packInfos.Add(info);
			}

			return packInfos;
		}

		public void Write(ResourcePackIdVersions packInfos)
		{
			if (packInfos == null || packInfos.Count == 0)
			{
				WriteUnsignedVarInt(0);
				return;
			}

			WriteUnsignedVarInt((uint)packInfos.Count);
			foreach (var info in packInfos)
			{
				Write(info.Id);
				Write(info.Version);
				Write(info.SubPackName);
			}
		}

		public ResourcePackIdVersions ReadResourcePackIdVersions()
		{
			uint count = ReadUnsignedVarInt();

			var packInfos = new ResourcePackIdVersions();
			for (int i = 0; i < count; i++)
			{
				var id = ReadString();
				var version = ReadString();
				var subPackName = ReadString();
				var info = new PackIdVersion
				{
					Id = id,
					Version = version,
					SubPackName = subPackName
				};
				packInfos.Add(info);
			}

			return packInfos;
		}

		public void Write(ResourcePackIds ids)
		{
			if (ids == null)
			{
				Write((short)0);
				return;
			}

			Write((short)ids.Count);

			foreach (var id in ids)
			{
				Write(id);
			}
		}

		public ResourcePackIds ReadResourcePackIds()
		{
			int count = ReadShort();

			var ids = new ResourcePackIds();
			for (int i = 0; i < count; i++)
			{
				var id = ReadString();
				ids.Add(id);
			}

			return ids;
		}

		public void Write(Skin skin)
		{
			Write(skin.SkinId);
			Write(skin.PlayFabId);
			Write(skin.ResourcePatch);
			Write(skin.Width);
			Write(skin.Height);
			WriteByteArray(skin.Data);

			if (skin.Animations?.Count > 0)
			{
				Write(skin.Animations.Count);
				foreach (Animation animation in skin.Animations)
				{
					Write(animation.ImageWidth);
					Write(animation.ImageHeight);
					WriteByteArray(animation.Image);
					Write(animation.Type);
					Write(animation.FrameCount);
					Write(animation.Expression);
				}
			}
			else
			{
				Write(0);
			}

			Write(skin.Cape.ImageWidth);
			Write(skin.Cape.ImageHeight);
			WriteByteArray(skin.Cape.Data);
			Write(skin.GeometryData);
			Write(skin.GeometryDataVersion);
			Write(skin.AnimationData);

			Write(skin.Cape.Id);
			Write(skin.SkinId + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
			Write(skin.ArmSize);
			Write(skin.SkinColor);
			Write(skin.PersonaPieces.Count);
			foreach (PersonaPiece piece in skin.PersonaPieces)
			{
				Write(piece.PieceId);
				Write(piece.PieceType);
				Write(piece.PackId);
				Write(piece.IsDefaultPiece);
				Write(piece.ProductId);
			}

			Write(skin.SkinPieces.Count);
			foreach (SkinPiece skinPiece in skin.SkinPieces)
			{
				Write(skinPiece.PieceType);
				Write(skinPiece.Colors.Count);
				foreach (string color in skinPiece.Colors)
				{
					Write(color);
				}
			}

			Write(skin.IsPremiumSkin);
			Write(skin.IsPersonaSkin);
			Write(skin.Cape.OnClassicSkin);
			Write(skin.IsPrimaryUser);
			Write(skin.isOverride);
		}

		public Skin ReadSkin()
		{
			Skin skin = new Skin();

			skin.SkinId = ReadString();
			skin.PlayFabId = ReadString();
			skin.ResourcePatch = ReadString();
			skin.Width = ReadInt();
			skin.Height = ReadInt();
			skin.Data = ReadByteArray(false);

			int animationCount = ReadInt();
			for (int i = 0; i < animationCount; i++)
			{
				skin.Animations.Add(
					new Animation()
					{
						ImageWidth = ReadInt(),
						ImageHeight = ReadInt(),
						Image = ReadByteArray(false),
						Type = ReadInt(),
						FrameCount = ReadFloat(),
						Expression = ReadInt()
					}
				);
			}

			skin.Cape.ImageWidth = ReadInt();
			skin.Cape.ImageHeight = ReadInt();
			skin.Cape.Data = ReadByteArray(false);
			skin.GeometryData = ReadString();
			skin.GeometryDataVersion = ReadString();
			skin.AnimationData = ReadString();

			skin.Cape.Id = ReadString();
			ReadString();
			skin.ArmSize = ReadString();
			skin.SkinColor = ReadString();
			int personaPieceCount = ReadInt();
			for (int i = 0; i < personaPieceCount; i++)
			{
				var p = new PersonaPiece();
				p.PieceId = ReadString();
				p.PieceType = ReadString();
				p.PackId = ReadString();
				p.IsDefaultPiece = ReadBool();
				p.ProductId = ReadString();
				skin.PersonaPieces.Add(p);
			}

			int skinPieceCount = ReadInt();
			for (int i = 0; i < skinPieceCount; i++)
			{
				var piece = new SkinPiece();
				piece.PieceType = ReadString();
				int colorAmount = ReadInt();
				for (int i2 = 0; i2 < colorAmount; i2++)
				{
					piece.Colors.Add(ReadString());
				}

				skin.SkinPieces.Add(piece);
			}

			skin.IsPremiumSkin = ReadBool();
			skin.IsPersonaSkin = ReadBool();
			skin.Cape.OnClassicSkin = ReadBool();
			skin.IsPrimaryUser = ReadBool();
			skin.isOverride = ReadBool();


			return skin;
		}

		const byte Shapeless = 0;
		const byte Shaped = 1;
		const byte Furnace = 2;
		const byte FurnaceData = 3;
		const byte Multi = 4;
		const byte ShulkerBox = 5;
		const byte ShapelessChemistry = 6;
		const byte ShapedChemistry = 7;
		const byte SmithingTransform = 8;
		const byte SmithingTrim = 9;

		public void Write(Recipes recipes)
		{
			WriteUnsignedVarInt((uint)recipes.Count);
			int UniqueId = 1;
			foreach (Recipe recipe in recipes)
			{
				switch (recipe)
				{
					case ShapelessRecipe shapelessRecipe:
					{
						WriteSignedVarInt(Shapeless);

						var rec = shapelessRecipe;
						var uuid = new UUID(Guid.NewGuid().ToString());
						Write($"{uuid}");
						WriteVarInt(rec.Input.Count);
						foreach (Item stack in rec.Input)
						{
							WriteRecipeIngredient(stack);
						}

						WriteVarInt(rec.Result.Count);
						foreach (Item item in rec.Result)
						{
							item.RuntimeId = 0;
							Write(item, false);
						}

						Write(rec.Id);
						Write(rec.Block);
						WriteSignedVarInt(0);
						Write((byte)1);
						WriteVarInt(UniqueId);

						break;
					}
					case ShapedRecipe shapedRecipe:
					{
						WriteSignedVarInt(Shaped);

						var rec = shapedRecipe;
						var uuid = new UUID(Guid.NewGuid().ToString());
						Write($"{uuid}");
						WriteSignedVarInt(rec.Width);
						WriteSignedVarInt(rec.Height);
						for (int w = 0; w < rec.Width; w++)
						{
							for (int h = 0; h < rec.Height; h++)
							{
								WriteRecipeIngredient(rec.Input[(h * rec.Width) + w]);
							}
						}

						WriteVarInt(rec.Result.Count);
						foreach (Item item in rec.Result)
						{
							item.RuntimeId = 0;
							Write(item, false);
						}

						Write(rec.Id);
						Write(rec.Block);
						WriteUnsignedVarInt(0);
						Write(true);
						Write((byte)1);
						WriteVarInt(UniqueId);

						break;
					}
					case SmeltingRecipe smeltingRecipe:
					{
						var rec = smeltingRecipe;
						if (rec.Input.Metadata == 0)
						{
							WriteSignedVarInt(Furnace);
							WriteSignedVarInt(rec.Input.Id);
							Write(rec.Result, false);
							Write(rec.Block);
						}
						else
						{
							WriteSignedVarInt(FurnaceData);
							WriteSignedVarInt(rec.Input.Id);
							WriteSignedVarInt(rec.Input.Metadata);
							Write(rec.Result, false);
							Write(rec.Block);
						}

						break;
					}
					case MultiRecipe multiRecipe:
					{
						WriteSignedVarInt(Multi);
						Write(recipe.Id);
						WriteVarInt(UniqueId);
						break;
					}
				}

				UniqueId++;
			}
		}

		public Recipes ReadRecipes()
		{
			var recipes = new Recipes();

			int count = (int)ReadUnsignedVarInt();

			for (int i = 0; i < count; i++)
			{
				int recipeType = ReadSignedVarInt();


				if (recipeType < 0)
				{
					Console.WriteLine("Read void recipe");
					break;
				}

				switch (recipeType)
				{
					case Shapeless:
					case ShulkerBox:
					{
						var recipe = new ShapelessRecipe();
						ReadString();
						int ingrediensCount = ReadVarInt();
						for (int j = 0; j < ingrediensCount; j++)
						{
							recipe.Input.Add(ReadRecipeData());
						}

						int resultCount = ReadVarInt();
						for (int j = 0; j < resultCount; j++)
						{
							recipe.Result.Add(ReadItem(false));
						}

						recipe.Id = ReadUUID();
						recipe.Block = ReadString();
						ReadSignedVarInt();
						var unlockReq = ReadByte();
						if (unlockReq == 0)
						{
							var ingredientCount = ReadVarInt();
							for (int a = 0; a < ingredientCount; a++)
							{
								ReadRecipeData();
							}
						}

						recipe.UniqueId = ReadVarInt();


						break;
					}
					case Shaped:
					{
						var uniqueid = ReadString();

						int width = ReadSignedVarInt();
						int height = ReadSignedVarInt();

						var recipe = new ShapedRecipe(width, height);
						if (width > 3 || height > 3)
							throw new Exception("Wrong number of ingredience. Width=" + width + ", height=" + height);
						for (int w = 0; w < width; w++)
						{
							for (int h = 0; h < height; h++)
							{
								recipe.Input[(h * width) + w] = ReadRecipeData();
							}
						}

						int resultCount = ReadVarInt();

						for (int j = 0; j < resultCount; j++)
						{
							recipe.Result.Add(ReadItem(false));
						}

						recipe.Id = ReadUUID();

						recipe.Block = ReadString();
						ReadUnsignedVarInt();
						var symetric = ReadBool();
						var unlockReq = ReadByte();
						if (unlockReq == 0)
						{
							var ingredientCount = ReadVarInt();
							for (int a = 0; a < ingredientCount; a++)
							{
								ReadRecipeData();
							}
						}

						recipe.UniqueId = ReadVarInt();
						recipes.Add(recipe);

						break;
					}
					case Furnace:
					{
						var recipe = new SmeltingRecipe();
						short id = (short)ReadSignedVarInt();

						Item result = ReadItem(false);
						recipe.Block = ReadString();
						recipe.Input = ItemFactory.GetItem(id, 0);
						recipe.Result = result;


						break;
					}
					case FurnaceData:
					{
						var recipe = new SmeltingRecipe();
						short id = (short)ReadSignedVarInt();
						short meta = (short)ReadSignedVarInt();
						Item result = ReadItem(false);
						recipe.Block = ReadString();
						recipe.Input = ItemFactory.GetItem(id, meta);
						recipe.Result = result;


						break;
					}
					case Multi:
					{
						var recipe = new MultiRecipe();
						recipe.Id = ReadUUID();
						recipe.UniqueId = ReadVarInt();

						break;
					}
					case ShapelessChemistry:
					{
						var recipe = new ShapelessRecipe();
						ReadString();
						int ingrediensCount = ReadVarInt();
						for (int j = 0; j < ingrediensCount; j++)
						{
							recipe.Input.Add(ReadRecipeData());
						}

						int resultCount = ReadVarInt();
						for (int j = 0; j < resultCount; j++)
						{
							recipe.Result.Add(ReadItem(false));
						}

						recipe.Id = ReadUUID();
						recipe.Block = ReadString();
						ReadSignedVarInt();
						recipe.UniqueId = ReadVarInt();


						break;
					}
					case ShapedChemistry:
					{
						ReadString();
						int width = ReadSignedVarInt();
						int height = ReadSignedVarInt();
						var recipe = new ShapedRecipe(width, height);
						if (width > 3 || height > 3)
							throw new Exception("Wrong number of ingredience. Width=" + width + ", height=" + height);
						for (int w = 0; w < width; w++)
						{
							for (int h = 0; h < height; h++)
							{
								recipe.Input[(h * width) + w] = ReadRecipeData();
							}
						}

						int resultCount = ReadVarInt();
						for (int j = 0; j < resultCount; j++)
						{
							recipe.Result.Add(ReadItem(false));
						}

						recipe.Id = ReadUUID();
						recipe.Block = ReadString();
						ReadSignedVarInt();
						recipe.UniqueId = ReadVarInt();

						break;
					}
					case SmithingTrim:
					{
						var recipe = new SmithingTrimRecipe();
						recipe.RecipeId = ReadString();
						recipe.Template = ReadRecipeData();
						recipe.Input = ReadRecipeData();
						recipe.Addition = ReadRecipeData();
						recipe.Block = ReadString();
						recipe.UniqueId = ReadVarInt();


						break;
					}
					case SmithingTransform:
					{
						var recipe = new SmithingTransformRecipe();
						recipe.RecipeId = ReadString();
						recipe.Template = ReadRecipeData();
						recipe.Input = ReadRecipeData();
						recipe.Addition = ReadRecipeData();
						recipe.Output = ReadItem(false);
						recipe.Block = ReadString();
						recipe.UniqueId = ReadVarInt();


						break;
					}
					default:
						Console.WriteLine($"Read unknown recipe type: {recipeType}");

						break;
				}
			}


			return recipes;
		}

		public void WriteRecipeIngredient(Item stack)
		{
			if (stack == null || stack.Id == 0)
			{
				Write(false);
				WriteVarInt(0);
				return;
			}

			Write(true);
			var translated = new TranslatedItem(0, 0);
			if (translated.Id != stack.Id)
			{
				Write((short)translated.Id);
				Write(translated.Meta);
			}
			else
			{
				Write(stack.Id);
				Write(stack.Metadata);
			}

			WriteSignedVarInt(stack.Count);
		}

		public Item ReadRecipeData()
		{
			short type = ReadByte();

			if (type == 1)
			{
				short id = ReadShort();
				short meta = ReadShort();
				short count = (short)ReadSignedVarInt();

				return ItemFactory.GetItem(id, meta, count);
			}
			else if (type == 2)
			{
				string expression = ReadString();
				int version = ReadByte();
				short count = (short)ReadSignedVarInt();

				return ItemFactory.GetItem(ItemFactory.GetItemIdByName(expression));
			}
			else if (type == 3)
			{
				string sId = ReadString();
				short count = (short)ReadSignedVarInt();

				return ItemFactory.GetItem(sId, 0, count);
			}
			else if (type == 4)
			{
				string sId = ReadString();
				short meta = ReadShort();

				return new ItemAir();
			}
			else if (type == 5)
			{
				string stri = ReadString();

				ItemFactory.GetItem(ItemFactory.GetItemIdByName(stri));
			}

			short coun = (short)ReadSignedVarInt();

			return new ItemAir();
		}

		public Item ReadShapedRecipeIngredient()
		{
			short type = ReadByte();
			if (type == -1)
			{
			}

			return new ItemAir();
		}


		public void Write(PotionContainerChangeRecipe[] recipes)
		{
			WriteSignedVarInt(0);
		}

		public PotionContainerChangeRecipe[] ReadPotionContainerChangeRecipes()
		{
			int count = (int)ReadUnsignedVarInt();
			var recipes = new PotionContainerChangeRecipe[count];
			for (int i = 0; i < recipes.Length; i++)
			{
				var recipe = new PotionContainerChangeRecipe();
				recipe.Input = ReadVarInt();
				recipe.Ingredient = ReadVarInt();
				recipe.Output = ReadVarInt();

				recipes[i] = recipe;
			}

			return recipes;
		}

		public void Write(MaterialReducerRecipe[] reducerRecipes)
		{
			WriteUnsignedVarInt((uint)reducerRecipes.Length);

			for (int i = 0; i < reducerRecipes.Length; i++)
			{
				var recipe = reducerRecipes[i];
				WriteVarInt((recipe.Input << 16) | recipe.InputMeta);
				WriteUnsignedVarInt((uint)recipe.Output.Length);

				foreach (var output in recipe.Output)
				{
					WriteVarInt(output.ItemId);
					WriteVarInt(output.ItemCount);
				}
			}
		}

		public MaterialReducerRecipe[] ReadMaterialReducerRecipes()
		{
			int count = (int)ReadUnsignedVarInt();
			var recipes = new MaterialReducerRecipe[count];
			for (int i = 0; i < recipes.Length; i++)
			{
				var inputIdAndMeta = ReadVarInt();
				var inputId = inputIdAndMeta >> 16;
				var inputMeta = inputIdAndMeta & 0x7fff;

				var outputCount = (int)ReadUnsignedVarInt();
				MaterialReducerRecipe.MaterialReducerRecipeOutput[] outputs =
					new MaterialReducerRecipe.MaterialReducerRecipeOutput[outputCount];

				for (int o = 0; o < outputs.Length; o++)
				{
					var itemId = ReadVarInt();
					var itemCount = ReadVarInt();

					outputs[o] = new MaterialReducerRecipe.MaterialReducerRecipeOutput(itemId, itemCount);
				}

				var recipe = new MaterialReducerRecipe(inputId, inputMeta, outputs);

				recipes[i] = recipe;
			}

			return recipes;
		}

		public void Write(PotionTypeRecipe[] recipes)
		{
			WriteSignedVarInt(0);
		}

		public PotionTypeRecipe[] ReadPotionTypeRecipes()
		{
			int count = (int)ReadUnsignedVarInt();
			var recipes = new PotionTypeRecipe[count];
			for (int i = 0; i < recipes.Length; i++)
			{
				var recipe = new PotionTypeRecipe();
				recipe.Input = ReadVarInt();
				recipe.InputMeta = ReadVarInt();
				recipe.Ingredient = ReadVarInt();
				recipe.IngredientMeta = ReadVarInt();
				recipe.Output = ReadVarInt();
				recipe.OutputMeta = ReadVarInt();

				recipes[i] = recipe;
			}

			return recipes;
		}


		const int MapUpdateFlagTexture = 0x02;
		const int MapUpdateFlagDecoration = 0x04;
		const int MapUpdateFlagInitialisation = 0x08;

		public void Write(MapInfo map)
		{
			WriteSignedVarLong(map.MapId);
			WriteUnsignedVarInt((uint)map.UpdateType);
			Write((byte)0);
			Write(false);
			Write(map.Origin);

			if ((map.UpdateType & MapUpdateFlagInitialisation) != 0)
			{
				WriteUnsignedVarInt(1);
				WriteSignedVarLong(map.MapId);
			}

			if ((map.UpdateType & (MapUpdateFlagInitialisation | MapUpdateFlagDecoration | MapUpdateFlagTexture)) != 0)
			{
				Write((byte)map.Scale);
			}

			if ((map.UpdateType & MapUpdateFlagDecoration) != 0)
			{
				var countTrackedObj = map.TrackedObjects.Length;

				WriteUnsignedVarInt((uint)countTrackedObj);
				foreach (var trackedObject in map.TrackedObjects)
				{
					if (trackedObject is EntityMapTrackedObject entity)
					{
						Write(0);
						WriteSignedVarLong(entity.EntityId);
					}
					else if (trackedObject is BlockMapTrackedObject block)
					{
						Write(1);
						Write(block.Coordinates);
					}
				}

				var count = map.Decorators.Length;

				WriteUnsignedVarInt((uint)count);
				foreach (var decorator in map.Decorators)
				{
					if (decorator is EntityMapDecorator entity)
					{
						WriteSignedVarLong(entity.EntityId);
					}
					else if (decorator is BlockMapDecorator block)
					{
						Write(block.Coordinates);
					}
				}

				WriteUnsignedVarInt((uint)count);
				foreach (var decorator in map.Decorators)
				{
					Write((byte)decorator.Icon);
					Write((byte)decorator.Rotation);
					Write((byte)decorator.X);
					Write((byte)decorator.Z);
					Write(decorator.Label);
					WriteUnsignedVarInt(decorator.Color);
				}
			}

			if ((map.UpdateType & MapUpdateFlagTexture) != 0)
			{
				WriteSignedVarInt(map.Col);
				WriteSignedVarInt(map.Row);

				WriteSignedVarInt(map.XOffset);
				WriteSignedVarInt(map.ZOffset);

				WriteUnsignedVarInt((uint)(map.Col * map.Row));
				int i = 0;
				for (int col = 0; col < map.Col; col++)
				{
					for (int row = 0; row < map.Row; row++)
					{
						byte r = map.Data[i++];
						byte g = map.Data[i++];
						byte b = map.Data[i++];
						byte a = map.Data[i++];
						uint color = BitConverter.ToUInt32(new byte[] { r, g, b, 0xff }, 0);
						WriteUnsignedVarInt(color);
					}
				}
			}
		}

		public MapInfo ReadMapInfo()
		{
			MapInfo map = new MapInfo();

			map.MapId = ReadSignedVarLong();
			map.UpdateType = (byte)ReadUnsignedVarInt();
			ReadByte();
			ReadBool();

			if ((map.UpdateType & MapUpdateFlagInitialisation) == MapUpdateFlagInitialisation)
			{
				var count = ReadUnsignedVarInt();
				for (int i = 0; i < count - 1; i++)
				{
					var eid = ReadSignedVarLong();
				}
			}

			if ((map.UpdateType & MapUpdateFlagTexture) == MapUpdateFlagTexture ||
			    (map.UpdateType & MapUpdateFlagDecoration) == MapUpdateFlagDecoration)
			{
				map.Scale = ReadByte();
			}

			if ((map.UpdateType & MapUpdateFlagDecoration) == MapUpdateFlagDecoration)
			{
				try
				{
					var entityCount = ReadUnsignedVarInt();
					for (int i = 0; i < entityCount; i++)
					{
						var type = ReadInt();
						if (type == 0)
						{
							var q = ReadSignedVarLong();
						}
						else if (type == 1)
						{
							var b = ReadBlockCoordinates();
						}
					}

					var count = ReadUnsignedVarInt();
					map.Decorators = new MapDecorator[count];
					for (int i = 0; i < count; i++)
					{
						MapDecorator decorator = new MapDecorator();
						decorator.Icon = ReadByte();
						decorator.Rotation = ReadByte();
						decorator.X = ReadByte();
						decorator.Z = ReadByte();
						decorator.Label = ReadString();
						decorator.Color = ReadUnsignedVarInt();
						map.Decorators[i] = decorator;
					}
				}
				catch (Exception e)
				{
				}
			}

			if ((map.UpdateType & MapUpdateFlagTexture) == MapUpdateFlagTexture)
			{
				try
				{
					map.Col = ReadSignedVarInt();
					map.Row = ReadSignedVarInt();

					map.XOffset = ReadSignedVarInt();
					map.ZOffset = ReadSignedVarInt();
					ReadUnsignedVarInt();
					for (int col = 0; col < map.Col; col++)
					{
						for (int row = 0; row < map.Row; row++)
						{
							ReadUnsignedVarInt();
						}
					}
				}
				catch (Exception e)
				{
					Console.WriteLine($"Errror while reading map data for map={map}", e);
				}
			}


			return map;
		}

		public pixelList ReadPixelList()
		{
			pixelList mapData = new pixelList();

			var listSize = ReadInt();
			for (int i = 0; i < listSize; i++)
			{
				mapData.mapData.Add(new pixelsData { pixel = ReadUnsignedVarInt(), index = ReadShort() });
			}

			return mapData;
		}

		public void Write(ScoreEntries list)
		{
			if (list == null) list = new ScoreEntries();

			Write((byte)(list.FirstOrDefault() is ScoreEntryRemove
				? McbeSetScore.Types.Remove
				: McbeSetScore.Types.Change));
			WriteUnsignedVarInt((uint)list.Count);
			foreach (var entry in list)
			{
				WriteSignedVarLong(entry.Id);
				Write(entry.ObjectiveName);
				Write(entry.Score);

				if (entry is ScoreEntryRemove)
				{
					continue;
				}

				if (entry is ScoreEntryChangePlayer player)
				{
					Write((byte)McbeSetScore.ChangeTypes.Player);
					WriteSignedVarLong(player.EntityId);
				}
				else if (entry is ScoreEntryChangeEntity entity)
				{
					Write((byte)McbeSetScore.ChangeTypes.Entity);
					WriteSignedVarLong(entity.EntityId);
				}
				else if (entry is ScoreEntryChangeFakePlayer fakePlayer)
				{
					Write((byte)McbeSetScore.ChangeTypes.FakePlayer);
					Write(fakePlayer.CustomName);
				}
			}
		}

		public ScoreEntries ReadScoreEntries()
		{
			var list = new ScoreEntries();
			byte type = ReadByte();
			var length = ReadUnsignedVarInt();
			for (var i = 0; i < length; ++i)
			{
				var entryId = ReadSignedVarLong();
				var entryObjectiveName = ReadString();
				var entryScore = ReadUint();

				ScoreEntry entry = null;

				if (type == (int)McbeSetScore.Types.Remove)
				{
					entry = new ScoreEntryRemove();
				}
				else
				{
					McbeSetScore.ChangeTypes changeType = (McbeSetScore.ChangeTypes)ReadByte();
					switch (changeType)
					{
						case McbeSetScore.ChangeTypes.Player:
							entry = new ScoreEntryChangePlayer { EntityId = ReadSignedVarLong() };
							break;
						case McbeSetScore.ChangeTypes.Entity:
							entry = new ScoreEntryChangeEntity { EntityId = ReadSignedVarLong() };
							break;
						case McbeSetScore.ChangeTypes.FakePlayer:
							entry = new ScoreEntryChangeFakePlayer { CustomName = ReadString() };
							break;
					}
				}

				if (entry == null) continue;

				entry.Id = entryId;
				entry.ObjectiveName = entryObjectiveName;
				entry.Score = entryScore;

				list.Add(entry);
			}

			return list;
		}

		public void Write(ScoreboardIdentityEntries list)
		{
			if (list == null) list = new ScoreboardIdentityEntries();

			Write((byte)(list.FirstOrDefault() is ScoreboardClearIdentityEntry
				? McbeSetScoreboardIdentity.Operations.ClearIdentity
				: McbeSetScoreboardIdentity.Operations.RegisterIdentity));
			WriteUnsignedVarInt((uint)list.Count);
			foreach (var entry in list)
			{
				WriteSignedVarLong(entry.Id);
				if (entry is ScoreboardRegisterIdentityEntry reg)
				{
					WriteSignedVarLong(reg.EntityId);
				}
			}
		}

		public ScoreboardIdentityEntries ReadScoreboardIdentityEntries()
		{
			ScoreboardIdentityEntries list = new ScoreboardIdentityEntries();

			McbeSetScoreboardIdentity.Operations type = (McbeSetScoreboardIdentity.Operations)ReadByte();
			var length = ReadUnsignedVarInt();
			for (var i = 0; i < length; ++i)
			{
				var scoreboardId = ReadSignedVarLong();

				switch (type)
				{
					case McbeSetScoreboardIdentity.Operations.RegisterIdentity:
						list.Add(new ScoreboardRegisterIdentityEntry()
						{
							Id = scoreboardId,
							EntityId = ReadSignedVarLong()
						});
						break;
					case McbeSetScoreboardIdentity.Operations.ClearIdentity:
						list.Add(new ScoreboardClearIdentityEntry() { Id = scoreboardId });
						break;
				}
			}

			return list;
		}

		public Experiments ReadExperiments()
		{
			Experiments experiments = new Experiments();
			var count = ReadInt();

			for (int i = 0; i < count; i++)
			{
				var experimentName = ReadString();
				var enabled = ReadBool();
				experiments.Add(new Experiments.Experiment(experimentName, enabled));
			}

			return experiments;
		}

		public void Write(Experiments experiments)
		{
			if (experiments == null)
			{
				Write(0);
				return;
			}

			Write(experiments.Count);

			foreach (var experiment in experiments)
			{
				Write(experiment.Name);
				Write(experiment.Enabled);
			}
		}

		public void Write(EducationUriResource resource)
		{
			Write(resource.ButtonName);
			Write(resource.LinkUri);
		}

		public EducationUriResource ReadEducationUriResource()
		{
			string name = ReadString();
			var uri = ReadString();

			return new EducationUriResource(name, uri);
		}

		public void Write(UpdateSubChunkBlocksPacketEntry entry)
		{
			Write(entry.Coordinates);
			WriteUnsignedVarInt(entry.BlockRuntimeId);
			WriteUnsignedVarInt(entry.Flags);
			WriteUnsignedVarLong(entry.SyncedUpdatedEntityUniqueId);
			WriteUnsignedVarInt(entry.SyncedUpdateType);
		}

		public UpdateSubChunkBlocksPacketEntry ReadUpdateSubChunkBlocksPacketEntry()
		{
			var entry = new UpdateSubChunkBlocksPacketEntry();
			entry.Coordinates = ReadBlockCoordinates();
			entry.BlockRuntimeId = ReadUnsignedVarInt();
			entry.Flags = ReadUnsignedVarInt();
			entry.SyncedUpdatedEntityUniqueId = ReadUnsignedVarLong();
			entry.SyncedUpdateType = ReadUnsignedVarInt();

			return entry;
		}

		public void Write(UpdateSubChunkBlocksPacketEntry[] entries)
		{
			WriteUnsignedVarInt((uint)entries.Length);
			foreach (var entry in entries)
				Write(entry);
		}

		public UpdateSubChunkBlocksPacketEntry[] ReadUpdateSubChunkBlocksPacketEntrys()
		{
			var count = ReadUnsignedVarInt();
			UpdateSubChunkBlocksPacketEntry[] entries = new UpdateSubChunkBlocksPacketEntry[(int)count];

			for (int i = 0; i < entries.Length; i++)
			{
				entries[i] = ReadUpdateSubChunkBlocksPacketEntry();
			}

			return entries;
		}

		public void Write(HeightMapData data)
		{
			if (data == null)
			{
				Write((byte)SubChunkPacketHeightMapType.NoData);

				return;
			}

			if (data.IsAllTooHigh)
			{
				Write((byte)SubChunkPacketHeightMapType.AllTooHigh);

				return;
			}

			if (data.IsAllTooLow)
			{
				Write((byte)SubChunkPacketHeightMapType.AllTooLow);

				return;
			}

			Write((byte)SubChunkPacketHeightMapType.Data);

			for (int i = 0; i < data.Heights.Length; i++)
			{
				Write((byte)data.Heights[i]);
			}
		}

		public HeightMapData ReadHeightMapData()
		{
			SubChunkPacketHeightMapType type = (SubChunkPacketHeightMapType)ReadByte();

			if (type != SubChunkPacketHeightMapType.Data)
				return null;

			short[] heights = new short[256];

			for (int i = 0; i < heights.Length; i++)
			{
				heights[i] = (short)ReadByte();
			}

			return new HeightMapData(heights);
		}

		public void Write(SubChunkPositionOffset offset)
		{
			Write(offset.XOffset);
			Write(offset.YOffset);
			Write(offset.ZOffset);
		}

		public SubChunkPositionOffset ReadSubChunkPositionOffset()
		{
			SubChunkPositionOffset result = new SubChunkPositionOffset();
			result.XOffset = ReadSByte();
			result.YOffset = ReadSByte();
			result.ZOffset = ReadSByte();
			return result;
		}

		public void Write(SubChunkPositionOffset[] offsets)
		{
			Write(offsets.Length);

			foreach (var offset in offsets)
			{
				Write(offset);
			}
		}

		public SubChunkPositionOffset[] ReadSubChunkPositionOffsets()
		{
			var count = ReadInt();
			SubChunkPositionOffset[] offsets = new SubChunkPositionOffset[count];

			for (int i = 0; i < offsets.Length; i++)
			{
				offsets[i] = ReadSubChunkPositionOffset();
			}

			return offsets;
		}

		public DimensionData ReadDimensionData()
		{
			DimensionData data = new DimensionData();
			data.Identifier = ReadString();
			data.MaxHeight = ReadVarInt();
			data.MinHeight = ReadVarInt();
			data.Generator = ReadVarInt();

			return data;
		}

		public void Write(DimensionData data)
		{
			Write(data.Identifier);
			WriteVarInt(data.MaxHeight);
			WriteVarInt(data.MinHeight);
			WriteVarInt(data.Generator);
		}

		public void Write(DimensionDefinitions definitions)
		{
			WriteUnsignedVarInt((uint)definitions.Count);

			foreach (var def in definitions)
			{
				Write(def.Key);
				Write(def.Value);
			}
		}

		public DimensionDefinitions ReadDimensionDefinitions()
		{
			DimensionDefinitions definitions = new DimensionDefinitions();

			var count = ReadUnsignedVarInt();
			for (int i = 0; i < count; i++)
			{
				var stringId = ReadString();
				var data = ReadDimensionData();

				definitions.TryAdd(stringId, data);
			}

			return definitions;
		}

		public void Write(PropertySyncData syncData)
		{
			if (syncData == null)
			{
				WriteUnsignedVarInt(0);
				WriteUnsignedVarInt(0);
				return;
			}

			WriteUnsignedVarInt((uint)syncData.intProperties.Count);

			foreach (var intP in syncData.intProperties)
			{
				WriteUnsignedVarInt(intP.Key);
				WriteSignedVarInt(intP.Value);
			}

			WriteUnsignedVarInt((uint)syncData.floatProperties.Count);

			foreach (var intF in syncData.floatProperties)
			{
				WriteUnsignedVarInt(intF.Key);
				Write(intF.Value);
			}
		}

		public PropertySyncData ReadPropertySyncData()
		{
			PropertySyncData syncData = new PropertySyncData();
			var countInt = ReadUnsignedVarInt();
			for (int i = 0; i < countInt; i++)
			{
				syncData.intProperties.Add(ReadUnsignedVarInt(), ReadVarInt());
			}

			var countFloat = ReadUnsignedVarInt();
			for (int i = 0; i < countFloat; i++)
			{
				syncData.floatProperties.Add(ReadUnsignedVarInt(), ReadFloat());
			}

			return syncData;
		}

		public EmoteIds ReadEmoteId()
		{
			EmoteIds Ids = new EmoteIds();
			var emoteCount = ReadUnsignedVarInt();
			for (int i = 0; i < (int)emoteCount; i++)
			{
				Ids.emoteId.Add(ReadUUID());
			}

			return Ids;
		}

		public void Write(EmoteIds Ids)
		{
			Write(Ids.emoteId.Count);
			foreach (var emoteIds in Ids.emoteId)
			{
				Write(emoteIds);
			}
		}


		public fogStack Read()
		{
			fogStack stack = new fogStack();
			var effectCount = ReadUnsignedVarInt();
			for (int i = 0; i < (int)effectCount; i++)
			{
				stack.fogList.Add(ReadString());
			}

			return stack;
		}

		public void Write(fogStack stack)
		{
			WriteUnsignedVarInt((uint)stack.fogList.Count);
			foreach (string effect in stack.fogList)
			{
				Write(effect);
			}
		}

		public void Write(Biome[] biomes)
		{
			WriteUnsignedVarInt((uint)biomes.Count());
			for (short i = 0; i < biomes.Count(); i++)
			{
				Write(i);
				Write(false);
				Write(biomes[i].Temperature);
				Write(biomes[i].Downfall);
				Write(biomes[i].RedSporeDensity);
				Write(biomes[i].BlueSporeDensity);
				Write(biomes[i].AshDensity);
				Write(biomes[i].WhiteAshDensity);
				Write(biomes[i].Depth);
				Write(biomes[i].Scale);
				Write(biomes[i].WaterColor);
				Write(biomes[i].Downfall > 0 ? true : false);
				Write(false);
				Write(false);
			}

			WriteUnsignedVarInt((uint)biomes.Count());
			foreach (Biome biome in biomes)
			{
				Write(biome.DefinitionName);
			}
		}

		public Biome[] ReadBiomes()
		{
			var biomeCount = ReadUnsignedVarInt();
			var biomes = new Biome[biomeCount];
			for (int i = 0; i < biomeCount; i++)
			{
				var biome = new Biome();
				if (ReadBool())
				{
					biome.Id = ReadShort();
				}

				biome.Temperature = ReadFloat();
				biome.Downfall = ReadFloat();

				biomes[i] = biome;
			}

			var biomeNameCount = ReadUnsignedVarInt();
			for (int i = 0; i < biomeNameCount; i++)
			{
				biomes[i].DefinitionName = ReadString();
			}

			return biomes;
		}


		#region BiomeWeight

		public void WriteBiomeWeight(BiomeWeight weight)
		{
			Write(weight.Biome);
			WriteUnsignedVarInt(weight.Weight);
		}

		public BiomeWeight ReadBiomeWeight()
		{
			short biome = ReadShort(false);
			uint weight = ReadUnsignedVarInt();
			return new BiomeWeight { Biome = biome, Weight = weight };
		}

		#endregion

		#region BiomeTemperatureWeight

		public void WriteBiomeTemperatureWeight(BiomeTemperatureWeight tempWeight)
		{
			WriteSignedVarInt(tempWeight.Temperature);
			WriteUnsignedVarInt(tempWeight.Weight);
		}

		public BiomeTemperatureWeight ReadBiomeTemperatureWeight()
		{
			int temperature = ReadSignedVarInt();
			uint weight = ReadUnsignedVarInt();
			return new BiomeTemperatureWeight { Temperature = temperature, Weight = weight };
		}

		#endregion

		#region BiomeConditionalTransformation

		public void WriteBiomeConditionalTransformation(BiomeConditionalTransformation transformation)
		{
			WriteSignedVarInt(transformation.WeightedBiomes?.Length ?? 0);
			if (transformation.WeightedBiomes != null)
			{
				foreach (var item in transformation.WeightedBiomes)
				{
					WriteBiomeWeight(item);
				}
			}

			Write(transformation.ConditionJSON);
			WriteUnsignedVarInt(transformation.MinPassingNeighbours);
		}

		public BiomeConditionalTransformation ReadBiomeConditionalTransformation()
		{
			int count = ReadSignedVarInt();
			BiomeWeight[] weightedBiomes = new BiomeWeight[count];
			for (int i = 0; i < count; i++)
			{
				weightedBiomes[i] = ReadBiomeWeight();
			}

			short conditionJson = ReadShort(false);
			uint minPassingNeighbours = ReadUnsignedVarInt();

			return new BiomeConditionalTransformation
			{
				WeightedBiomes = weightedBiomes,
				ConditionJSON = conditionJson,
				MinPassingNeighbours = minPassingNeighbours
			};
		}

		#endregion

		#region BiomeMultiNoiseRules

		public void WriteBiomeMultiNoiseRules(BiomeMultiNoiseRules rules)
		{
			Write(rules.Temperature);
			Write(rules.Humidity);
			Write(rules.Altitude);
			Write(rules.Weirdness);
			Write(rules.Weight);
		}

		public BiomeMultiNoiseRules ReadBiomeMultiNoiseRules()
		{
			float temperature = ReadFloat();
			float humidity = ReadFloat();
			float altitude = ReadFloat();
			float weirdness = ReadFloat();
			float weight = ReadFloat();

			return new BiomeMultiNoiseRules
			{
				Temperature = temperature,
				Humidity = humidity,
				Altitude = altitude,
				Weirdness = weirdness,
				Weight = weight
			};
		}

		#endregion

		#region BiomeOverworldRules

		public void WriteBiomeOverworldRules(BiomeOverworldRules rules)
		{
			WriteSignedVarInt(rules.HillsTransformations?.Length ?? 0);
			if (rules.HillsTransformations != null)
				foreach (var item in rules.HillsTransformations)
					WriteBiomeWeight(item);

			WriteSignedVarInt(rules.MutateTransformations?.Length ?? 0);
			if (rules.MutateTransformations != null)
				foreach (var item in rules.MutateTransformations)
					WriteBiomeWeight(item);

			WriteSignedVarInt(rules.RiverTransformations?.Length ?? 0);
			if (rules.RiverTransformations != null)
				foreach (var item in rules.RiverTransformations)
					WriteBiomeWeight(item);

			WriteSignedVarInt(rules.ShoreTransformations?.Length ?? 0);
			if (rules.ShoreTransformations != null)
				foreach (var item in rules.ShoreTransformations)
					WriteBiomeWeight(item);

			WriteSignedVarInt(rules.PreHillsEdgeTransformations?.Length ?? 0);
			if (rules.PreHillsEdgeTransformations != null)
				foreach (var item in rules.PreHillsEdgeTransformations)
					WriteBiomeConditionalTransformation(item);

			WriteSignedVarInt(rules.PostShoreEdgeTransformations?.Length ?? 0);
			if (rules.PostShoreEdgeTransformations != null)
				foreach (var item in rules.PostShoreEdgeTransformations)
					WriteBiomeConditionalTransformation(item);

			WriteSignedVarInt(rules.ClimateTransformations?.Length ?? 0);
			if (rules.ClimateTransformations != null)
				foreach (var item in rules.ClimateTransformations)
					WriteBiomeTemperatureWeight(item);
		}

		public BiomeOverworldRules ReadBiomeOverworldRules()
		{
			int count1 = ReadSignedVarInt();
			BiomeWeight[] hills = new BiomeWeight[count1];
			for (int i = 0; i < count1; i++) hills[i] = ReadBiomeWeight();

			int count2 = ReadSignedVarInt();
			BiomeWeight[] mutate = new BiomeWeight[count2];
			for (int i = 0; i < count2; i++) mutate[i] = ReadBiomeWeight();

			int count3 = ReadSignedVarInt();
			BiomeWeight[] river = new BiomeWeight[count3];
			for (int i = 0; i < count3; i++) river[i] = ReadBiomeWeight();

			int count4 = ReadSignedVarInt();
			BiomeWeight[] shore = new BiomeWeight[count4];
			for (int i = 0; i < count4; i++) shore[i] = ReadBiomeWeight();

			int count5 = ReadSignedVarInt();
			BiomeConditionalTransformation[] preHills = new BiomeConditionalTransformation[count5];
			for (int i = 0; i < count5; i++) preHills[i] = ReadBiomeConditionalTransformation();

			int count6 = ReadSignedVarInt();
			BiomeConditionalTransformation[] postShore = new BiomeConditionalTransformation[count6];
			for (int i = 0; i < count6; i++) postShore[i] = ReadBiomeConditionalTransformation();

			int count7 = ReadSignedVarInt();
			BiomeTemperatureWeight[] climate = new BiomeTemperatureWeight[count7];
			for (int i = 0; i < count7; i++) climate[i] = ReadBiomeTemperatureWeight();

			return new BiomeOverworldRules
			{
				HillsTransformations = hills,
				MutateTransformations = mutate,
				RiverTransformations = river,
				ShoreTransformations = shore,
				PreHillsEdgeTransformations = preHills,
				PostShoreEdgeTransformations = postShore,
				ClimateTransformations = climate
			};
		}

		#endregion

		#region BiomeCappedSurface

		public void WriteBiomeCappedSurface(BiomeCappedSurface surface)
		{
			WriteSignedVarInt(surface.FloorBlocks?.Length ?? 0);
			if (surface.FloorBlocks != null)
				foreach (var item in surface.FloorBlocks)
					Write(item, false);


			WriteSignedVarInt(surface.CeilingBlocks?.Length ?? 0);
			if (surface.CeilingBlocks != null)
				foreach (var item in surface.CeilingBlocks)
					Write(item, false);


			Write(surface.SeaBlock.HasValue);
			if (surface.SeaBlock.HasValue) WriteUnsignedVarInt(surface.SeaBlock.Value);


			Write(surface.FoundationBlock.HasValue);
			if (surface.FoundationBlock.HasValue) WriteUnsignedVarInt(surface.FoundationBlock.Value);


			Write(surface.BeachBlock.HasValue);
			if (surface.BeachBlock.HasValue) WriteUnsignedVarInt(surface.BeachBlock.Value);
		}

		public BiomeCappedSurface ReadBiomeCappedSurface()
		{
			int count1 = ReadSignedVarInt();
			int[] floor = new int[count1];
			for (int i = 0; i < count1; i++) floor[i] = ReadInt(false);


			int count2 = ReadSignedVarInt();
			int[] ceiling = new int[count2];
			for (int i = 0; i < count2; i++) ceiling[i] = ReadInt(false);


			bool hasSea = ReadBool();
			Optional<uint> seaBlock = new Optional<uint>();
			if (hasSea) seaBlock = new Optional<uint>(ReadUnsignedVarInt());


			bool hasFoundation = ReadBool();
			Optional<uint> foundationBlock = new Optional<uint>();
			if (hasFoundation) foundationBlock = new Optional<uint>(ReadUnsignedVarInt());


			bool hasBeach = ReadBool();
			Optional<uint> beachBlock = new Optional<uint>();
			if (hasBeach) beachBlock = new Optional<uint>(ReadUnsignedVarInt());

			return new BiomeCappedSurface
			{
				FloorBlocks = floor,
				CeilingBlocks = ceiling,
				SeaBlock = seaBlock,
				FoundationBlock = foundationBlock,
				BeachBlock = beachBlock
			};
		}

		#endregion

		#region BiomeMesaSurface

		public void WriteBiomeMesaSurface(BiomeMesaSurface mesa)
		{
			WriteUnsignedVarInt(mesa.ClayMaterial);
			WriteUnsignedVarInt(mesa.HardClayMaterial);
			Write(mesa.BrycePillars);
			Write(mesa.HasForest);
		}

		public BiomeMesaSurface ReadBiomeMesaSurface()
		{
			uint clay = ReadUnsignedVarInt();
			uint hardClay = ReadUnsignedVarInt();
			bool bryce = ReadBool();
			bool forest = ReadBool();

			return new BiomeMesaSurface
			{
				ClayMaterial = clay,
				HardClayMaterial = hardClay,
				BrycePillars = bryce,
				HasForest = forest
			};
		}

		#endregion

		#region BiomeSurfaceMaterial

		public void WriteBiomeSurfaceMaterial(BiomeSurfaceMaterial material)
		{
			Write(material.TopBlock, false);
			Write(material.MidBlock, false);
			Write(material.SeaFloorBlock, false);
			Write(material.FoundationBlock, false);
			Write(material.SeaBlock, false);
			Write(material.SeaFloorDepth, false);
		}

		public BiomeSurfaceMaterial ReadBiomeSurfaceMaterial()
		{
			int top = ReadInt(false);
			int mid = ReadInt(false);
			int seaFloor = ReadInt(false);
			int foundation = ReadInt(false);
			int sea = ReadInt(false);
			int seaFloorDepth = ReadInt(false);

			return new BiomeSurfaceMaterial
			{
				TopBlock = top,
				MidBlock = mid,
				SeaFloorBlock = seaFloor,
				FoundationBlock = foundation,
				SeaBlock = sea,
				SeaFloorDepth = seaFloorDepth
			};
		}

		#endregion

		#region BiomeElementData

		public void WriteBiomeElementData(BiomeElementData data)
		{
			Write(data.NoiseFrequencyScale);
			Write(data.NoiseLowerBound);
			Write(data.NoiseUpperBound);
			WriteSignedVarInt(data.HeightMinType);
			Write(data.HeightMin);
			WriteSignedVarInt(data.HeightMaxType);
			Write(data.HeightMax);
			WriteBiomeSurfaceMaterial(data.AdjustedMaterials);
		}

		public BiomeElementData ReadBiomeElementData()
		{
			float freq = ReadFloat();
			float lower = ReadFloat();
			float upper = ReadFloat();
			int minHeightType = ReadSignedVarInt();
			short minHeight = ReadShort(false);
			int maxHeightType = ReadSignedVarInt();
			short maxHeight = ReadShort(false);
			BiomeSurfaceMaterial materials = ReadBiomeSurfaceMaterial();

			return new BiomeElementData
			{
				NoiseFrequencyScale = freq,
				NoiseLowerBound = lower,
				NoiseUpperBound = upper,
				HeightMinType = minHeightType,
				HeightMin = minHeight,
				HeightMaxType = maxHeightType,
				HeightMax = maxHeight,
				AdjustedMaterials = materials
			};
		}

		#endregion

		#region BiomeMountainParameters

		public void WriteBiomeMountainParameters(BiomeMountainParameters parameters)
		{
			Write(parameters.SteepBlock, false);
			Write(parameters.NorthSlopes);
			Write(parameters.SouthSlopes);
			Write(parameters.WestSlopes);
			Write(parameters.EastSlopes);
			Write(parameters.TopSlideEnabled);
		}

		public BiomeMountainParameters ReadBiomeMountainParameters()
		{
			int steep = ReadInt(false);
			bool north = ReadBool();
			bool south = ReadBool();
			bool west = ReadBool();
			bool east = ReadBool();
			bool topSlide = ReadBool();

			return new BiomeMountainParameters
			{
				SteepBlock = steep,
				NorthSlopes = north,
				SouthSlopes = south,
				WestSlopes = west,
				EastSlopes = east,
				TopSlideEnabled = topSlide
			};
		}

		#endregion

		#region BiomeCoordinate

		public void WriteBiomeCoordinate(BiomeCoordinate coord)
		{
			WriteSignedVarInt(coord.MinValueType);
			Write(coord.MinValue);
			WriteSignedVarInt(coord.MaxValueType);
			Write(coord.MaxValue);
			WriteUnsignedVarInt(coord.GridOffset);
			WriteUnsignedVarInt(coord.GridStepSize);
			WriteSignedVarInt(coord.Distribution);
		}

		public BiomeCoordinate ReadBiomeCoordinate()
		{
			int minType = ReadSignedVarInt();
			short minVal = ReadShort(false);
			int maxType = ReadSignedVarInt();
			short maxVal = ReadShort(false);
			uint gridOffset = ReadUnsignedVarInt();
			uint gridStep = ReadUnsignedVarInt();
			int dist = ReadSignedVarInt();

			return new BiomeCoordinate
			{
				MinValueType = minType,
				MinValue = minVal,
				MaxValueType = maxType,
				MaxValue = maxVal,
				GridOffset = gridOffset,
				GridStepSize = gridStep,
				Distribution = dist
			};
		}

		#endregion

		#region BiomeScatterParameter

		public void WriteBiomeScatterParameter(BiomeScatterParameter param)
		{
			WriteSignedVarInt(param.Coordinates?.Length ?? 0);
			if (param.Coordinates != null)
				foreach (var item in param.Coordinates)
					WriteBiomeCoordinate(item);

			WriteSignedVarInt(param.EvaluationOrder);
			WriteSignedVarInt(param.ChancePercentType);
			Write(param.ChancePercent);
			Write(param.ChanceNumerator, false);
			Write(param.ChanceDenominator, false);
			WriteSignedVarInt(param.IterationsType);
			Write(param.Iterations);
		}

		public BiomeScatterParameter ReadBiomeScatterParameter()
		{
			int count = ReadSignedVarInt();
			BiomeCoordinate[] coords = new BiomeCoordinate[count];
			for (int i = 0; i < count; i++)
			{
				coords[i] = ReadBiomeCoordinate();
			}

			int evalOrder = ReadSignedVarInt();
			int chanceType = ReadSignedVarInt();
			short chancePercent = ReadShort(false);
			int chanceNum = ReadInt(false);
			int chanceDen = ReadInt(false);
			int iterType = ReadSignedVarInt();
			short iter = ReadShort(false);

			return new BiomeScatterParameter
			{
				Coordinates = coords,
				EvaluationOrder = evalOrder,
				ChancePercentType = chanceType,
				ChancePercent = chancePercent,
				ChanceNumerator = chanceNum,
				ChanceDenominator = chanceDen,
				IterationsType = iterType,
				Iterations = iter
			};
		}

		#endregion

		#region BiomeConsolidatedFeature

		public void WriteBiomeConsolidatedFeature(BiomeConsolidatedFeature feature)
		{
			WriteBiomeScatterParameter(feature.Scatter);
			Write(feature.Feature);
			Write(feature.Identifier);
			Write(feature.Pass);
			Write(feature.CanUseInternal);
		}

		public BiomeConsolidatedFeature ReadBiomeConsolidatedFeature()
		{
			BiomeScatterParameter scatter = ReadBiomeScatterParameter();
			short feat = ReadShort(false);
			short id = ReadShort(false);
			short pass = ReadShort(false);
			bool canUse = ReadBool();

			return new BiomeConsolidatedFeature
			{
				Scatter = scatter,
				Feature = feat,
				Identifier = id,
				Pass = pass,
				CanUseInternal = canUse
			};
		}

		#endregion

		#region BiomeClimate

		public void WriteBiomeClimate(BiomeClimate climate)
		{
			Write(climate.Temperature);
			Write(climate.Downfall);
			Write(climate.RedSporeDensity);
			Write(climate.BlueSporeDensity);
			Write(climate.AshDensity);
			Write(climate.WhiteAshDensity);
			Write(climate.SnowAccumulationMin);
			Write(climate.SnowAccumulationMax);
		}

		public BiomeClimate ReadBiomeClimate()
		{
			float temp = ReadFloat();
			float down = ReadFloat();
			float red = ReadFloat();
			float blue = ReadFloat();
			float ash = ReadFloat();
			float whiteAsh = ReadFloat();
			float snowMin = ReadFloat();
			float snowMax = ReadFloat();

			return new BiomeClimate
			{
				Temperature = temp,
				Downfall = down,
				RedSporeDensity = red,
				BlueSporeDensity = blue,
				AshDensity = ash,
				WhiteAshDensity = whiteAsh,
				SnowAccumulationMin = snowMin,
				SnowAccumulationMax = snowMax
			};
		}

		#endregion

		#region BiomeChunkGeneration

		public void WriteBiomeChunkGeneration(BiomeChunkGeneration generation)
		{
			Write(generation.Climate.HasValue);
			if (generation.Climate.HasValue) WriteBiomeClimate(generation.Climate.Value);


			Write(generation.ConsolidatedFeatures.HasValue);
			if (generation.ConsolidatedFeatures.HasValue)
			{
				WriteUnsignedVarInt((uint)(generation.ConsolidatedFeatures.Value?.Length ?? 0));
				if (generation.ConsolidatedFeatures.Value != null)
					foreach (var item in generation.ConsolidatedFeatures.Value)
						WriteBiomeConsolidatedFeature(item);
			}


			Write(generation.MountainParameters.HasValue);
			if (generation.MountainParameters.HasValue)
				WriteBiomeMountainParameters(generation.MountainParameters.Value);


			Write(generation.SurfaceMaterialAdjustments.HasValue);
			if (generation.SurfaceMaterialAdjustments.HasValue)
			{
				WriteUnsignedVarInt((uint)(generation.SurfaceMaterialAdjustments.Value?.Length ?? 0));
				if (generation.SurfaceMaterialAdjustments.Value != null)
					foreach (var item in generation.SurfaceMaterialAdjustments.Value)
						WriteBiomeElementData(item);
			}


			Write(generation.SurfaceMaterials.HasValue);
			if (generation.SurfaceMaterials.HasValue) WriteBiomeSurfaceMaterial(generation.SurfaceMaterials.Value);

			Write(generation.HasSwampSurface);
			Write(generation.HasFrozenOceanSurface);
			Write(generation.HasEndSurface);


			Write(generation.MesaSurface.HasValue);
			if (generation.MesaSurface.HasValue) WriteBiomeMesaSurface(generation.MesaSurface.Value);


			Write(generation.CappedSurface.HasValue);
			if (generation.CappedSurface.HasValue) WriteBiomeCappedSurface(generation.CappedSurface.Value);


			Write(generation.OverworldRules.HasValue);
			if (generation.OverworldRules.HasValue) WriteBiomeOverworldRules(generation.OverworldRules.Value);


			Write(generation.MultiNoiseRules.HasValue);
			if (generation.MultiNoiseRules.HasValue) WriteBiomeMultiNoiseRules(generation.MultiNoiseRules.Value);


			Write(generation.LegacyRules.HasValue);
			if (generation.LegacyRules.HasValue)
			{
				WriteUnsignedVarInt((uint)(generation.LegacyRules.Value?.Length ?? 0));
				if (generation.LegacyRules.Value != null)
					foreach (var item in generation.LegacyRules.Value)
						WriteBiomeConditionalTransformation(item);
			}

			Write(generation.ReplacementsData.HasValue);
			if (generation.ReplacementsData.HasValue)
			{
				WriteUnsignedVarInt((uint)(generation.ReplacementsData.Value?.Length ?? 0));
				if (generation.ReplacementsData.Value != null)
					foreach (var item in generation.ReplacementsData.Value)
						WriteBiomeReplacementData(item);
			}
		}

		public void WriteBiomeReplacementData(BiomeReplacementData value)
		{
			Write(value.Biome);
			Write(value.Dimension);
			WriteSignedVarInt(value.TargetBiomes?.Length ?? 0);
			if (value.TargetBiomes != null)
			{
				foreach (var item in value.TargetBiomes)
				{
					Write(item);
				}
			}

			Write(value.Amount);
			Write(value.NoiseFrequencyScale);
			Write(value.ReplacementIndex);
		}

		public BiomeReplacementData ReadBiomeReplacementData()
		{
			var data = new BiomeReplacementData();

			data.Biome = ReadShort();
			data.Dimension = ReadShort();


			int targetBiomesLength = ReadSignedVarInt();
			if (targetBiomesLength > 0)
			{
				data.TargetBiomes = new short[targetBiomesLength];
				for (int i = 0; i < targetBiomesLength; i++)
				{
					data.TargetBiomes[i] = ReadShort();
				}
			}
			else
			{
				data.TargetBiomes = Array.Empty<short>();
			}

			data.Amount = ReadFloat();
			data.NoiseFrequencyScale = ReadFloat();
			data.ReplacementIndex = ReadUint();

			return data;
		}

		public BiomeChunkGeneration ReadBiomeChunkGeneration()
		{
			bool hasClimate = ReadBool();
			Optional<BiomeClimate> climate = new Optional<BiomeClimate>();
			if (hasClimate) climate = new Optional<BiomeClimate>(ReadBiomeClimate());


			bool hasConsolidated = ReadBool();
			Optional<BiomeConsolidatedFeature[]> consolidated = new Optional<BiomeConsolidatedFeature[]>();
			if (hasConsolidated)
			{
				uint count = ReadUnsignedVarInt();
				BiomeConsolidatedFeature[] features = new BiomeConsolidatedFeature[count];
				for (int i = 0; i < count; i++)
				{
					features[i] = ReadBiomeConsolidatedFeature();
				}

				consolidated = new Optional<BiomeConsolidatedFeature[]>(features);
			}


			bool hasMountain = ReadBool();
			Optional<BiomeMountainParameters> mountain = new Optional<BiomeMountainParameters>();
			if (hasMountain) mountain = new Optional<BiomeMountainParameters>(ReadBiomeMountainParameters());


			bool hasSurfaceAdj = ReadBool();
			Optional<BiomeElementData[]> surfaceAdj = new Optional<BiomeElementData[]>();
			if (hasSurfaceAdj)
			{
				uint count = ReadUnsignedVarInt();
				BiomeElementData[] data = new BiomeElementData[count];
				for (int i = 0; i < count; i++)
				{
					data[i] = ReadBiomeElementData();
				}

				surfaceAdj = new Optional<BiomeElementData[]>(data);
			}


			bool hasSurfaceMat = ReadBool();
			Optional<BiomeSurfaceMaterial> surfaceMat = new Optional<BiomeSurfaceMaterial>();
			if (hasSurfaceMat) surfaceMat = new Optional<BiomeSurfaceMaterial>(ReadBiomeSurfaceMaterial());

			bool swamp = ReadBool();
			bool frozenOcean = ReadBool();
			bool end = ReadBool();


			bool hasMesa = ReadBool();
			Optional<BiomeMesaSurface> mesa = new Optional<BiomeMesaSurface>();
			if (hasMesa) mesa = new Optional<BiomeMesaSurface>(ReadBiomeMesaSurface());


			bool hasCapped = ReadBool();
			Optional<BiomeCappedSurface> capped = new Optional<BiomeCappedSurface>();
			if (hasCapped) capped = new Optional<BiomeCappedSurface>(ReadBiomeCappedSurface());


			bool hasOverworld = ReadBool();
			Optional<BiomeOverworldRules> overworld = new Optional<BiomeOverworldRules>();
			if (hasOverworld) overworld = new Optional<BiomeOverworldRules>(ReadBiomeOverworldRules());


			bool hasMultiNoise = ReadBool();
			Optional<BiomeMultiNoiseRules> multiNoise = new Optional<BiomeMultiNoiseRules>();
			if (hasMultiNoise) multiNoise = new Optional<BiomeMultiNoiseRules>(ReadBiomeMultiNoiseRules());


			bool hasLegacy = ReadBool();
			Optional<BiomeConditionalTransformation[]> legacy = new Optional<BiomeConditionalTransformation[]>();
			if (hasLegacy)
			{
				uint count = ReadUnsignedVarInt();
				BiomeConditionalTransformation[] rules = new BiomeConditionalTransformation[count];
				for (int i = 0; i < count; i++)
				{
					rules[i] = ReadBiomeConditionalTransformation();
				}

				legacy = new Optional<BiomeConditionalTransformation[]>(rules);
			}

			bool hasReplaceMent = ReadBool();
			Optional<BiomeReplacementData[]> replacement = new Optional<BiomeReplacementData[]>();
			if (hasReplaceMent)
			{
				uint count = ReadUnsignedVarInt();
				BiomeReplacementData[] rules = new BiomeReplacementData[count];
				for (int i = 0; i < count; i++)
				{
					rules[i] = ReadBiomeReplacementData();
				}

				replacement = new Optional<BiomeReplacementData[]>(rules);
			}

			return new BiomeChunkGeneration
			{
				Climate = climate,
				ConsolidatedFeatures = consolidated,
				MountainParameters = mountain,
				SurfaceMaterialAdjustments = surfaceAdj,
				SurfaceMaterials = surfaceMat,
				HasSwampSurface = swamp,
				HasFrozenOceanSurface = frozenOcean,
				HasEndSurface = end,
				MesaSurface = mesa,
				CappedSurface = capped,
				OverworldRules = overworld,
				MultiNoiseRules = multiNoise,
				LegacyRules = legacy,
				ReplacementsData = replacement
			};
		}

		#endregion

		#region BiomeDefinition

		public void WriteBiomeDefinition(BiomeDefinition definition)
		{
			Write(definition.NameIndex);
			Write(definition.BiomeID);
			Write(definition.Temperature);
			Write(definition.Downfall);
			Write(definition.RedSporeDensity);
			Write(definition.BlueSporeDensity);
			Write(definition.AshDensity);
			Write(definition.WhiteAshDensity);
			Write(definition.Depth);
			Write(definition.Scale);
			Write(definition.MapWaterColour, false);
			Write(definition.Rain);


			Write(definition.Tags.HasValue);
			if (definition.Tags.HasValue)
			{
				WriteUnsignedVarInt((uint)(definition.Tags.Value?.Length ?? 0));
				if (definition.Tags.Value != null)
					foreach (var item in definition.Tags.Value)
						Write(item);
			}


			Write(definition.ChunkGeneration.HasValue);
			if (definition.ChunkGeneration.HasValue)
			{
				WriteBiomeChunkGeneration(definition.ChunkGeneration.Value);
			}
		}

		public BiomeDefinition ReadBiomeDefinition()
		{
			short nameIndex = ReadShort(false);
			short biomeId = ReadShort(false);
			float temp = ReadFloat();
			float down = ReadFloat();
			float red = ReadFloat();
			float blue = ReadFloat();
			float ash = ReadFloat();
			float whiteAsh = ReadFloat();
			float depth = ReadFloat();
			float scale = ReadFloat();
			int mapWater = ReadInt(false);
			bool rain = ReadBool();


			bool hasTags = ReadBool();
			Optional<ushort[]> tags = new Optional<ushort[]>();
			if (hasTags)
			{
				uint count = ReadUnsignedVarInt();
				ushort[] tagArray = new ushort[count];
				for (int i = 0; i < count; i++)
				{
					tagArray[i] = ReadUshort(false);
				}

				tags = new Optional<ushort[]>(tagArray);
			}


			bool hasChunkGen = ReadBool();
			Optional<BiomeChunkGeneration> chunkGen = new Optional<BiomeChunkGeneration>();
			if (hasChunkGen)
			{
				chunkGen = new Optional<BiomeChunkGeneration>(ReadBiomeChunkGeneration());
			}

			return new BiomeDefinition
			{
				NameIndex = nameIndex,
				BiomeID = biomeId,
				Temperature = temp,
				Downfall = down,
				RedSporeDensity = red,
				BlueSporeDensity = blue,
				AshDensity = ash,
				WhiteAshDensity = whiteAsh,
				Depth = depth,
				Scale = scale,
				MapWaterColour = mapWater,
				Rain = rain,
				Tags = tags,
				ChunkGeneration = chunkGen
			};
		}

		#endregion

		public bool CanRead()
		{
			return _reader.Position < _reader.Length;
		}

		public void SetEncodedMessage(byte[] encodedMessage)
		{
			_encodedMessage = encodedMessage;
		}

		public virtual void Reset()
		{
			ResetPacket();
			_encodedMessage = null;
			Bytes = null;
			_writer?.Close();
			_reader?.Close();
			_buffer?.Close();
			_writer = null;
			_reader = null;
			_buffer = null;
		}

		protected virtual void ResetPacket()
		{
		}

		private object _encodeSync = new object();

		private static RecyclableMemoryStreamManager _streamManager = new RecyclableMemoryStreamManager();
		private static ConcurrentDictionary<int, bool> _isLob = new ConcurrentDictionary<int, bool>();

		public virtual byte[] Encode()
		{
			byte[] cache = _encodedMessage;
			if (cache != null) return cache;

			lock (_encodeSync)
			{
				if (_encodedMessage != null) return _encodedMessage;


				bool isLob = _isLob.ContainsKey(Id);
				_buffer = isLob ? _streamManager.GetStream() : new MemoryStream();
				using (_writer = new BinaryWriter(_buffer, Encoding.UTF8, true))
				{
					EncodePacket();

					_writer.Flush();


					var buffer = (MemoryStream)_buffer;
					_encodedMessage = buffer.ToArray();
					if (!isLob && _encodedMessage.Length >= 85_000)
					{
						_isLob.TryAdd(Id, true);
					}
				}

				_buffer.Dispose();

				_writer = null;
				_buffer = null;

				return _encodedMessage;
			}
		}

		protected virtual void EncodePacket()
		{
			_buffer.Position = 0;
			if (IsMcbe) WriteVarInt(Id);
			else Write((byte)Id);
		}

		[Obsolete("Use decode with ReadOnlyMemory<byte> instead.")]
		public virtual Packet Decode(byte[] buffer)
		{
			return Decode(new ReadOnlyMemory<byte>(buffer));
		}

		public virtual Packet Decode(ReadOnlyMemory<byte> buffer)
		{
			Bytes = buffer;
			_reader = new MemoryStreamReader(buffer);

			DecodePacket();


			_reader.Dispose();
			_reader = null;

			return this;
		}

		protected virtual void DecodePacket()
		{
			Id = IsMcbe ? ReadVarInt() : ReadByte();
		}

		public static string HexDump(ReadOnlyMemory<byte> bytes, int bytesPerLine = 16, bool printLineCount = false)
		{
			return HexDump(bytes.Span, bytesPerLine, printLineCount);
		}

		private static string HexDump(ReadOnlySpan<byte> bytes, in int bytesPerLine, in bool printLineCount)
		{
			var sb = new StringBuilder();
			for (int line = 0; line < bytes.Length; line += bytesPerLine)
			{
				byte[] lineBytes = bytes.Slice(line).ToArray().Take(bytesPerLine).ToArray();
				if (printLineCount) sb.AppendFormat("{0:x8} ", line);
				sb.Append(string.Join(" ", lineBytes.Select(b => b.ToString("x2"))
						.ToArray())
					.PadRight(bytesPerLine * 3));
				sb.Append(" ");
				sb.Append(new string(lineBytes.Select(b => b < 32 ? '.' : (char)b)
					.ToArray()));
				sb.AppendLine();
			}

			return sb.ToString();
		}
	}
}