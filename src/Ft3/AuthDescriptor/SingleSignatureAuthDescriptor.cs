using System.Collections.Generic;
using System.Linq;
using Chromia.Postchain.Client.GTX;

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
            var gtv = new List<dynamic>(){
                Util.AuthTypeToString(AuthType.SingleSig),
                new List<string>(){Util.ByteArrayToString(this.PubKey)}.ToArray(),
                new List<dynamic>(){this.Flags.ToGTV(), Util.ByteArrayToString(this.PubKey)}
            };
            return gtv.ToArray();
        }

        public byte[] Hash()
        {
            var gtv = new List<dynamic>(){
                Util.AuthTypeToString(AuthType.SingleSig),
                new List<byte[]>(){this.PubKey}.ToArray(),
                new List<dynamic>(){this.Flags.ToGTV(), Util.ByteArrayToString(this.PubKey)}
            };
            return Gtx.ArgToGTXValue(gtv).Encode();
        }
    }
}