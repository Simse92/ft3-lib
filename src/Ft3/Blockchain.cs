using System.Threading.Tasks;
using System;
using System.Collections.Generic;

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
            return await Account.GetByParticipantId(id, this.NewSession(user));
        }

        public async Task<Account[]> GetAccountsByAuthDescriptorId(byte[] id, User user)
        {
            return await Account.GetByAuthDescriptorId(id, this.NewSession(user));
        }

        public async Task<Account> RegisterAccount(AuthDescriptor authDescriptor, User user)
        {
            return await Account.Register(authDescriptor, this.NewSession(user));
        }

        public async Task<Asset[]> GetAssetsByName(string name)
        {
            return await Asset.GetByName(name, this);
        }

        public async void LinkChain(byte[] chainId)
        {
            var tx = this.Connection.Gtx.NewTransaction(new byte[][] {});
            tx.AddOperation("ft3.link_chain", Util.ByteArrayToString(chainId));
            await tx.PostAndWaitConfirmation();
        }

        public async Task<bool> IsLinkedWithChain(byte[] chainId)
        {
            return await this.Query(
                "ft3.is_linked_with_chain",
                ("chain_rid", Util.ByteArrayToString(chainId))
            ) == 1;
        }

        public async Task<List<byte[]>> GetLinkedChainsIds()
        {
            var linkedChains = await this.Query("ft3.get_linked_chains");
            var chainIds = new List<byte[]>();

            foreach (var linkedChain in linkedChains)
            {
                chainIds.Add(Util.HexStringToBuffer(linkedChain["chainId"]));
            }

            return chainIds;
        }

        public async Task<Blockchain[]> GetLinkedChains()
        {
            var chainIds = await this.GetLinkedChainsIds();
            // ToDo
        }

        public async Task<dynamic> Query(string name, params dynamic[] queryObject)
        {
            return await this.Connection.Query(name, queryObject);
        }

        public async Task<dynamic> Call(User user, params GtvSerializable[] args)
        {
            var txBuilder = this.CreateTransactionBuilder();
            // ToDo
        }

        public TransactionBuilder CreateTransactionBuilder()
        {
            return new TransactionBuilder(this);
        }
    }
}