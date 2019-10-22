using System.Threading.Tasks;

namespace Chromia.Postchain.Ft3
{
    public class Transaction
    {
        private readonly Client.GTX.Transaction _tx;
        private readonly Blockchain _blockchain;

        public Transaction(Client.GTX.Transaction tx, Blockchain blockchain)
        {
            this._tx = tx;
            this._blockchain = blockchain;
        }

        public Transaction Sign(KeyPair keyPair)
        {
            this._tx.Sign(keyPair.PrivKey, keyPair.PubKey);
            return this;
        }

        public async Task<dynamic> Post()
        {
            return await this._tx.PostAndWaitConfirmation();
        }
    }

}