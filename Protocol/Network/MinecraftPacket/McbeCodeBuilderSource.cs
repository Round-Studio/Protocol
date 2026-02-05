namespace Protocol.Network.MinecraftPacket;

public static class CodeBuilderOperation
{
	public const byte None = 0;


	public const byte Get = 1;


	public const byte Set = 2;


	public const byte Reset = 3;
}

public static class CodeBuilderCategory
{
	public const byte None = 0;


	public const byte Status = 1;


	public const byte Instantiation = 2;
}

public static class CodeBuilderStatus
{
	public const byte None = 0;


	public const byte NotStarted = 1;


	public const byte InProgress = 2;


	public const byte Paused = 3;


	public const byte Error = 4;


	public const byte Succeeded = 5;
}

public class McpeCodeBuilderSource : Packet
{
	public McpeCodeBuilderSource()
	{
		Id = 178;
		IsMcpe = true;
	}


	public byte Operation { get; set; }


	public byte Category { get; set; }


	public byte CodeStatus { get; set; }


	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(Operation);


		Write(Category);


		Write(CodeStatus);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		Operation = ReadByte();


		Category = ReadByte();


		CodeStatus = ReadByte();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();
		Operation = CodeBuilderOperation.None;
		Category = CodeBuilderCategory.None;
		CodeStatus = CodeBuilderStatus.None;
	}
}