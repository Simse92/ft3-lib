using System.Collections.Generic;

namespace Chromia.Postchain.Ft3
{
    public class AuthDescriptorFactory
    {
        public AuthDescriptor Create(AuthType type, byte[] args)
        {
            switch(type)
            {
                case AuthType.SingleSig: 
                    return this.CreateSingleSig(args);
            }
            return null;
        }

        private SingleSignatureAuthDescriptor CreateSingleSig(byte[] args)
        {   
            // ToDo
            // var decodedDescriptor = gtv.decodeGtv(args);
            dynamic[] decodedDescriptor = {};
            var flags = new List<FlagsType>();

            foreach (var flag in decodedDescriptor[0])
            {
                flags.Add(Util.StringToFlagType((string) flag));
            }
            
            return new SingleSignatureAuthDescriptor(
                Util.HexStringToBuffer((string) decodedDescriptor[1]),
                flags.ToArray()
            );
        }
    }
}