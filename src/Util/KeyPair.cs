using System.Collections.Generic;

namespace Chromia.Postchain.Ft3.Util
{
    public class KeyPair
    {
        public readonly byte[] PubKey;
        public readonly byte[] PrivKey;

        public KeyPair(string privateKey = null)
        {
            if(privateKey != null)
            {
                this.PrivKey = Util.HexStringToBuffer(privateKey);
                this.PubKey = Client.Util.VerifyKeyPair(privateKey);
            }
            else
            {
                var keyPair = Client.Util.MakeKeyPair();
                this.PubKey = keyPair["pubKey"];
                this.PubKey = keyPair["privKey"];
            }
        }

        public Dictionary<string, byte[]> MakeKeyPair()
        {
            return Client.Util.MakeKeyPair();
        }

    }

}