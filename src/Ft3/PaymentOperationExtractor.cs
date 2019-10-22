namespace Chromia.Postchain.Ft3
{
    public class PaymentOperationExtractor
    {
        private readonly byte[] _transaction;
        private readonly string _chainId;

        public PaymentOperationExtractor(byte[] rawTransaction, string chainId)
        {
            this._transaction = rawTransaction;
            this._chainId = chainId;
        }

        public PaymentOperation[] Extract()
        {
            // ToDO
            return null;
        }
    }
}