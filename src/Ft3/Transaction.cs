using Chromia.Postchain.Client;
using System.Threading.Tasks;

namespace Chromia.Postchain.Ft3
{
    public class Transaction
    {
        private readonly Client.Transaction _tx;
        private readonly Blockchain _blockchain;

        public Transaction(Client.Transaction tx, Blockchain blockchain)
        {
            this._tx = tx;
            this._blockchain = blockchain;
        }

        public Transaction Sign(KeyPair keyPair)
        {
            this._tx.Sign(keyPair.PrivKey, keyPair.PubKey);
            return this;
        }

        public async Task<PostchainErrorControl> Post()
        {
            return await this._tx.PostAndWaitConfirmation();
        }

        // public byte[] Raw()
        // {
        //     return Util.HexStringToBuffer(_tx.Encode());
        // }
    }

}