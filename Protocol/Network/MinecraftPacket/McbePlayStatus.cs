namespace Protocol.Network.MinecraftPacket;

public class McbePlayStatus : Packet
{
	public enum PlayStatus
	{
		LoginSuccess = 0,
		LoginFailedClient = 1,
		LoginFailedServer = 2,
		PlayerSpawn = 3,
		LoginFailedInvalidTenant = 4,
		LoginFailedVanillaEdu = 5,
		LoginFailedEduVanilla = 6,
		LoginFailedServerFull = 7
	}

	public int status;

	public McbePlayStatus()
	{
		Id = 0x02;
		IsMcbe = true;
	}

	protected override void EncodePacket()
	{
		base.EncodePacket();


		WriteBe(status);
	}


	protected override void DecodePacket()
	{
		base.DecodePacket();


		status = ReadIntBe();
	}


	protected override void ResetPacket()
	{
		base.ResetPacket();

		status = default;
	}
}