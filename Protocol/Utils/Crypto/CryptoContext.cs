using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Crypto;

namespace Protocol.Utils.Crypto
{
	public class CryptoContext
	{
		public bool UseEncryption;

		//public RijndaelManaged Algorithm { get; set; }

		public IBufferedCipher Decryptor { get; set; }
		//public MemoryStream InputStream { get; set; }
		//public CryptoStream CryptoStreamIn { get; set; }

		public IBufferedCipher Encryptor { get; set; }
		//public MemoryStream OutputStream { get; set; }
		//public CryptoStream CryptoStreamOut { get; set; }

		public long SendCounter = -1;

		public AsymmetricCipherKeyPair ClientKey { get; set; }
		public byte[] Key { get; set; }
	}
}
