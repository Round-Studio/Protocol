namespace Protocol.Network.MinecraftPacket;

public class McpeDeathInfo : Packet
{
	public McpeDeathInfo()
	{
		Id = 189;
		IsMcpe = true;
	}


	public string Cause { get; set; } = string.Empty;


	public string[] Messages { get; set; } = new string[0];


	protected override void EncodePacket()
	{
		base.EncodePacket();


		Write(Cause);


		WriteUnsignedVarInt((uint)(Messages?.Length ?? 0));

		if (Messages != null)
			foreach (var message in Messages)

				Write(message ?? string.Empty);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		Cause = ReadString();


		var count = ReadUnsignedVarInt();

		Messages = new string[count];
		for (var i = 0; i < count; i++)

			Messages[i] = ReadString();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();
		Cause = string.Empty;
		Messages = new string[0];
	}
}