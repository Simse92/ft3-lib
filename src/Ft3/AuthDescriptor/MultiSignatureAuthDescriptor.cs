using Chromia.Postchain.Client.GTX;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Chromia.Postchain.Ft3
{
    public class MultiSignatureAuthDescriptor : AuthDescriptor
    {
        public List<byte[]> PubKeys;
        public Flags Flags;
        public int SignatureRequired;

        public MultiSignatureAuthDescriptor(List<byte[]> pubkeys, int signatureRequired, FlagsType[] flags)
        {
            if(signatureRequired > pubkeys.Count)
            {
                throw new Exception("Number of required signatures have to be less or equal to number of pubkeys");
            }

            this.PubKeys = pubkeys;
            this.SignatureRequired = signatureRequired;
            this.Flags = new Flags(flags.ToList());
        }

        public List<byte[]> GetSigners()
        {
            return this.PubKeys;
        }

        public byte[] GetId()
        {
            return this.Hash();
        }
        
        public dynamic[] ToGTV()
        {
            var hexPubs = new List<string>();
            foreach (var pubkey in this.PubKeys)
            {
                hexPubs.Add(Util.ByteArrayToString(pubkey));
            }

            var gtv = new List<dynamic>(){
                Util.AuthTypeToString(AuthType.MultiSig),
                hexPubs.ToArray(),
                new List<dynamic>()
                {
                    this.Flags.ToGTV(),
                    this.SignatureRequired,
                    hexPubs.ToArray()
                }
            };
            return gtv.ToArray();
        }

        public byte[] Hash()
        {
            var hexPubs = new List<string>();
            foreach (var pubkey in this.PubKeys)
            {
                hexPubs.Add(Util.ByteArrayToString(pubkey));
            }


            var gtv = new List<dynamic>(){
                Util.AuthTypeToString(AuthType.MultiSig),
                this.PubKeys.ToArray(),
                new List<dynamic>()
                {
                    this.Flags.ToGTV(),
                    this.SignatureRequired,
                    hexPubs.ToArray()
                }
            };

            return Gtx.ArgToGTXValue(gtv).Encode();
        }
    }
}