namespace Chromia.Postchain.Ft3
{
    public class TransactionBuilder
    {
        public readonly Blockchain Blockchain;
        public TransactionBuilder(Blockchain blockchain)
        {
            this.Blockchain = blockchain;
        }
    }
}