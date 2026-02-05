using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol.Network
{
	public class PackageManager
	{
		public Dictionary<int,Lazy<Packet>> base_packets = new Dictionary<int,Lazy<Packet>>();
		public PackageManager()
		{
			
		}
		public Packet GetPacket(int id)
		{

			return null;
		}
	}
}
