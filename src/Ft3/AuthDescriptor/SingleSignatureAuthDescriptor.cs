using System.Linq;
using System.Collections.Generic;

namespace Chromia.Postchain.Ft3
{
    public class SingleSignatureAuthDescriptor : AuthDescriptor
    {
        public byte[] PubKey;
        public Flags Flags;

        public SingleSignatureAuthDescriptor(byte[] pubKey, FlagsType[] flags)
        {
            this.Flags = new Flags(flags.ToList());
            this.PubKey = pubKey;
        }

        public List<byte[]> GetSigners()
        {
            return new List<byte[]>(){this.PubKey};
        }

        public byte[] GetId()
        {
            return this.Hash();
        }

        public dynamic[] ToGTV()
        {
            return null;
        }

        public byte[] Hash()
        {
            return null;
        }
    }
}