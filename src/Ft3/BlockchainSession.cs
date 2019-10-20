using System.Threading.Tasks;
using System.Collections.Generic;

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

        public async Task<Account> GetAccountById(byte[] id)
        {
            return await Account.GetById(id, this);
        }

        public async Task<List<Account>> GetAccountsByParticipantId(byte[] id)
        {
            return await Account.GetByParticipantId(id, this);
        }

        public async Task<List<Account>> GetAccountsByAuthDescriptor(byte[] id)
        {
            return await Account.GetByAuthDescriptorId(id, this);
        }

        public async Task<dynamic> Query(string name, params dynamic[] queryObject)
        {
            return await this.Blockchain.Query(name, queryObject);
        }

        // async call(...args: Array<GtvSerializable>): Promise<any> {
        //     return await this.blockchain.call(this.user, ...args);
        // }
    }
}