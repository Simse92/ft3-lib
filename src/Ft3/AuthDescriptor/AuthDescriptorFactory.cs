using Chromia.Postchain.Client.GTV;

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
            // var decodedDescriptor = gtv.decodeGtv(args);
            // return new SingleSignatureAuthDescriptor(
            // Buffer.from(decodedDescriptor[1], 'hex'),
            // decodedDescriptor[0]
            // );
            return null;
        }
    }
}