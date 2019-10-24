using Chromia.Postchain.Ft3;
using System.Collections.Generic;

public class TestUser
{
    public static User SingleSig()
    {
        KeyPair keyPair = new KeyPair();
        SingleSignatureAuthDescriptor singleSigAuthDescriptor = new SingleSignatureAuthDescriptor(
            keyPair.PubKey,
            new List<FlagsType>(){FlagsType.Account, FlagsType.Transfer}.ToArray()
        );
        return new User(keyPair, singleSigAuthDescriptor);
    }
}