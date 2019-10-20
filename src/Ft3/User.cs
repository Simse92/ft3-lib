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

        public static User GenerateSingleSigUser()
        {
            return null;
        }
    }

}