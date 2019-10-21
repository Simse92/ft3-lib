using System.Collections.Generic;
using System.Linq;

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
            // ToDo
            return null;
        }

        public byte[] Hash()
        {
            // ToDo
            return null;
        }
    }
}