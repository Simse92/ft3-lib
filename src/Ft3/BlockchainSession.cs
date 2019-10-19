namespace Chromia.Postchain.Ft3
{
    public class BlockchainSession
    {
        public readonly User User;
        public readonly Blockchain Blockchain;

        public BlockchainSession(User user, Blockchain blockchain)
        {  
            this.User = user;
            this.Blockchain = blockchain;
        }
    }
}