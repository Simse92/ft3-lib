using System.Collections.Generic;

namespace Chromia.Postchain.Ft3
{
    public class User
    {
        public KeyPair KeyPair;
        public AuthDescriptor AuthDescriptor;

        
        public User(KeyPair keyPair, AuthDescriptor authDescriptor)
        {
            this.KeyPair = keyPair;
            this.AuthDescriptor = authDescriptor;
        }

        public static User GenerateSingleSigUser(FlagsType[] flags = null)
        {
            if(flags == null)
            {
                var defaultFlags = new List<FlagsType>(){FlagsType.Account, FlagsType.Transfer};
                flags = defaultFlags.ToArray();
            }

            var keyPair = new KeyPair();

            return new User(
                keyPair,
                new SingleSignatureAuthDescriptor(
                    keyPair.PubKey,
                    flags
                )
            );
        }
    }
}