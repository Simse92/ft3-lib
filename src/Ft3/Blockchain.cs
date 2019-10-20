using System.Threading.Tasks;
using System;

namespace Chromia.Postchain.Ft3
{
    public class Blockchain
    {
        public readonly byte[] Id;
        public readonly BlockchainInfo Info;
        public readonly ConnectionClient Connection;
        private readonly DirectoryService _directoryService;

        public Blockchain(byte[] id, BlockchainInfo info, ConnectionClient connection, DirectoryService directoryService)
        {
            this.Id = id;
            this.Info = info;
            this.Connection = connection;
            this._directoryService = directoryService;
        }

        public async static Task<Blockchain> Initialize(byte[] blockchainRID, DirectoryService directoryService)
        {
            var chainConnectionInfo = directoryService.GetChainConnectionInfo(blockchainRID);

            if(chainConnectionInfo == null)
            {
                throw new Exception("Cannot find details for chain with RID: " + Util.ByteArrayToString(blockchainRID));
            }

            var connection = new ConnectionClient(
                chainConnectionInfo.Url, 
                Util.ByteArrayToString(blockchainRID)
            );

            var info = await BlockchainInfo.GetInfo(connection);
            return new Blockchain(blockchainRID, info, connection, directoryService);
        }

        public BlockchainSession NewSession(User user)
        {
            return new BlockchainSession(user, this);
        }

        public async Task<Account[]> GetAccountsByParticipantId(byte[] id, User user)
        {
            // return await Account.get
        }






        public async Task<dynamic> Query(string name, params dynamic[] queryObject)
        {
            return await this.Connection.Query(name, queryObject);
        }
    }

}