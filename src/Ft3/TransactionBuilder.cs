namespace Chromia.Postchain.Ft3
{
    public class TransactionBuilder
    {
        public readonly BlockchainInfo Blockchain;
        public TransactionBuilder(Blockchain blockchain)
        {
            this.Blockchain = blockchain;
        }
    }
}