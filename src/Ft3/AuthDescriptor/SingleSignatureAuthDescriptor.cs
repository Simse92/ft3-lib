using System.Collections.Generic;
using System.Linq;
using Chromia.Postchain.Client.GTV;

namespace Chromia.Postchain.Ft3
{
    public class SingleSignatureAuthDescriptor : AuthDescriptor
    {
        private byte[] _pubkey;
        public Flags Flags;
        public readonly IAuthdescriptorRule AuthRule;

        public SingleSignatureAuthDescriptor(byte[] pubKey, FlagsType[] flags, IAuthdescriptorRule rule = null)
        {
            this._pubkey = pubKey;
            this.Flags = new Flags(flags.ToList());
            this.AuthRule = rule;
        }

        public List<byte[]> Signers
        {
            get => new List<byte[]>(){this._pubkey};
        }

        public List<byte[]> PubKey
        {
            get => new List<byte[]>() {this._pubkey};
        }

        public byte[] ID
        {
            get => this.Hash();
        }

        public IAuthdescriptorRule Rule
        {
            get => this.AuthRule;
        }

        public dynamic[] ToGTV()
        {
            var gtv = new List<dynamic>(){
                Util.AuthTypeToString(AuthType.SingleSig),
                new List<string>(){Util.ByteArrayToString(this._pubkey)}.ToArray(),
                new List<dynamic>(){this.Flags.ToGTV(), Util.ByteArrayToString(this._pubkey)}.ToArray(),
                this.AuthRule?.ToGTV()
            };

            return gtv.ToArray();
        }

        public byte[] Hash()
        {
            var gtv = new List<dynamic>(){
                Util.AuthTypeToString(AuthType.SingleSig),
                new List<byte[]>(){this._pubkey}.ToArray(),
                new List<dynamic>(){this.Flags.ToGTV(), Util.ByteArrayToString(this._pubkey)}.ToArray(),
                this.AuthRule?.ToGTV()
                
            }.ToArray();
            
            return Gtv.Hash(gtv);
        }
    }
}