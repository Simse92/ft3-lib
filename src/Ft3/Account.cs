using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Chromia.Postchain.Ft3
{
    public enum AuthType
    {
        SingleSig,
        MultiSig
    }

    public enum FlagsType
    {

        None,
        Account,
        Transfer
    }

    public class Flags
    {
        private List<FlagsType> FlagsOrder = new List<FlagsType>(){FlagsType.Account, FlagsType.Transfer};
        public List<FlagsType> FlagList;

        public Flags(List<FlagsType> flags)
        {
            this.FlagList = flags;
        }

        public bool HasFlag(FlagsType flag)
        {
            return this.FlagList.Contains(flag);
        }

        public dynamic[] ToGTV()
        {
            var validFlags = new List<string>();
            foreach (var flag in this.FlagList)
            {
                if(this.FlagsOrder.Contains(flag))
                {
                    validFlags.Add(Util.FlagTypeToString(flag));
                }
            }

            return validFlags.ToArray();
        }
    }

    public interface AuthDescriptor
    {
        byte[] Hash();
        List<byte[]> GetSigners();
        byte[] GetId();
        dynamic[] ToGTV();
    }

    public class Account
    {
        private PaymentHistorySyncManager _paymentHistorySyncManager = new PaymentHistorySyncManager();
        public readonly byte[] Id;
        public List<AuthDescriptor> AuthDescriptor;
        public List<AssetBalance> Assets = new List<AssetBalance>();
        public readonly BlockchainSession Session;

        public Account(byte[] id, AuthDescriptor[] authDescriptor, BlockchainSession session)
        {
            this.Id = id;
            this.AuthDescriptor = authDescriptor.ToList();
            this.Session = session;
        }

        public Blockchain GetBlockchain()
        {
            return this.Session.Blockchain;
        }

        public static async Task<Account[]> GetByParticipantId(byte[] id, BlockchainSession session)
        {
            var accountIds = await session.Query(
                "ft3.get_accounts_by_participant_id",
                ("id", Util.ByteArrayToString(id))
            );

            var idList = new List<byte[]>();
            foreach (var accountId in accountIds)
            {
                idList.Add(Util.HexStringToBuffer(accountId["id"]));
            }

            return await Account.GetByIds(idList, session);
        }

        public static async Task<Account[]> GetByAuthDescriptorId(byte[] id, BlockchainSession session)
        {
            var accountIds = await session.Query(
                "ft3.get_accounts_by_auth_descriptor_id",
                ("descriptor_id", Util.ByteArrayToString(id))
            );

            var idList = new List<byte[]>();
            foreach (var accountId in accountIds)
            {
                idList.Add(Util.HexStringToBuffer(accountId["id"]));
            }

            return await Account.GetByIds(idList, session);
        }

        public static async Task<Account> Register(AuthDescriptor authDescriptor, BlockchainSession session)
        {
            await session.Call(Account.RegisterOp(authDescriptor).ToArray());
            var account = new Account(authDescriptor.Hash(), new List<AuthDescriptor>{authDescriptor}.ToArray(), session);
            await account.SyncAssets();
            return account;
        }
        public static async Task<Account[]> GetByIds(List<byte[]> ids, BlockchainSession session)
        {
            var accounts = new List<Account>();
            foreach (var id in ids)
            {
                var account = await Account.GetById(id, session);
                accounts.Add(account);
            }

            return accounts.ToArray();
        }

        public static async Task<Account> GetById(byte[] id, BlockchainSession session)
        {
            var account = await session.Query(
                "ft3.get_account_by_id",
                ("id", Util.ByteArrayToString(id))
            );

            if(account == null)
            {
                return null;
            }

            var authDescriptors = await session.Query(
                "ft3.get_account_auth_descriptors",
                ("id", Util.ByteArrayToString(id))
            );

            var authDescriptorFactory = new AuthDescriptorFactory();
            List<AuthDescriptor> authList = new List<AuthDescriptor>();

            foreach (var authDescriptor in authDescriptors)
            {
                authList.Add(
                    authDescriptorFactory.Create(
                        authDescriptor["type"],
                        Util.HexStringToBuffer(authDescriptor["args"])
                    )
                );
            }
            
            var acc = new Account(id, authList.ToArray(), session);
            await acc.SyncAssets();
            return acc;
        }

        public async void AddAuthDescriptor(AuthDescriptor authDescriptor)
        {
            await this.Session.Call(AddAuthDescriptorOp(authDescriptor).ToArray());
            this.AuthDescriptor.Add(authDescriptor);
        }

        public async Task DeleteAllAuthDescriptorsExclude(AuthDescriptor authDescriptor)
        {
            await this.Session.Call(
                "ft3.delete_all_auth_descriptors_exclude",
                this.Id,
                authDescriptor.GetId()
            );
        }

        private async Task SyncAssets()
        {
            this.Assets = await AssetBalance.GetByAccountId(this.Id, this.Session.Blockchain);
        }

        public AssetBalance GetAssetById(byte[] id)
        {
            return this.Assets.Find(assetBalance => assetBalance.Asset.GetId().Equals(id));
        }

        public async Task TransferInputsToOutputs(List<dynamic> inputs, List<dynamic> outputs)
        {
            var transactionBuilder = this.GetBlockchain().CreateTransactionBuilder();
            transactionBuilder.AddOperation("ft3.transfer", inputs, outputs);
            transactionBuilder.AddOperation("nop", new Random().Next().ToString());
            var tx = transactionBuilder.Build(this.Session.User.AuthDescriptor.GetSigners());
            tx.Sign(this.Session.User.KeyPair);
            await tx.Post();
            await this.SyncAssets();
        }

        public async void Transfer(byte[] accountId, byte[] assetId, float amount)
        {
            var input = new List<dynamic>{
                this.Id,
                assetId,
                this.AuthDescriptor[0].Hash(),
                amount,
                new dynamic[] {}
            };

            var output = new List<dynamic>{
                accountId,
                assetId,
                amount,
                new dynamic[] {}
            };

            await this.TransferInputsToOutputs(input, output);
        }

        public async void BurnTokens(byte[] assetId, float amount)
        {
            var input = new List<dynamic>(){
                this.Id,
                assetId,
                this.AuthDescriptor[0].Hash(),
                amount,
                new dynamic[] {}
            };

            await this.TransferInputsToOutputs(input, new List<dynamic>{});
        }

        public async Task<dynamic> GetPaymentHistory()
        {
            return await PaymentHistory.GetAccountById(this.Id, this.Session.Blockchain.Connection);
        }

        public async Task<PaymentHistoryIterator> GetPaymentHistoryIterator(int pageSize)
        {
            if(pageSize < 1)
            {
                throw new Exception("Page size has to be greater than 1");
            }
            await this._paymentHistorySyncManager.SyncAccount(this.Id, this.Session.Blockchain);
            return this._paymentHistorySyncManager.PaymentHistoryStore.GetIterator(this.Id, pageSize);
        }

        public async Task XcTransfer(byte[] destinationChainId, byte[] destinationAccountId, byte[] assetId, float amount)
        {
            var transactionBuilder = this.GetBlockchain().CreateTransactionBuilder();
            transactionBuilder.AddOperation(XcTransferOp(destinationChainId, destinationAccountId, assetId, amount));
            transactionBuilder.AddOperation("nop", new Random().Next().ToString());
            var tx = transactionBuilder.Build(this.Session.User.AuthDescriptor.GetSigners());
            tx.Sign(this.Session.User.KeyPair);
            await tx.Post();
            await this.SyncAssets();
        }

        /* Operation and query */
        public dynamic[] XcTransferOp(byte[] destinationChainId, byte[] destinationAccountId, byte[] assetId, float amount)
        {
            var gtv = new List<dynamic>() {
                "ft3.xc.init_xfer",
                new List<dynamic>() {
                    this.Id,
                    assetId,
                    this.Session.User.AuthDescriptor.GetId(),
                    amount,
                    new dynamic[] {}
                }.ToArray(),
                new List<dynamic>() {
                    destinationAccountId,
                    new dynamic[] {}
                }.ToArray(),
                new List<byte[]>(){
                    destinationChainId
                }.ToArray()
            };
            return gtv.ToArray();
        }

        public dynamic[] AddAuthDescriptorOp(AuthDescriptor authDescriptor)
        {
            var gtv = new List<dynamic>() {
                "ft3.add_auth_descriptor",
                this.Id,
                this.Session.User.AuthDescriptor.GetId(),
                authDescriptor
            };

            return gtv.ToArray();
        }

        public static dynamic[] RegisterOp(AuthDescriptor authDescriptor)
        {
            var gtv = new List<dynamic>() {
                "ft3.dev_register_account",
                authDescriptor
            };

            return gtv.ToArray();
        }
    }
}