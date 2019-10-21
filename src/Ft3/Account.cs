using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Chromia.Postchain.Ft3
{
    public enum AuthType
    {
        SingleSig,
        MultiSig
    }

    public enum FlagsType
    {
        Account,
        Transfer
    }

    public interface GtvSerializable
    {
        dynamic[] ToGTV();
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

        public void ToGTV()
        {
            // ToDo
        }
    }

    public interface AuthDescriptor : GtvSerializable
    {
        byte[] Hash();
    }

    public class Account
    {
        // private paymentHistorySyncManager = new PaymentHistorySyncManager();
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
            // ToDo
            // await session.call(...this.registerOp(authDescriptor));
            // const account = new Account(authDescriptor.hash(), [authDescriptor], session);
            // await account.syncAssets();
            return null;
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
            // ToDo
        }

        public async void DeleteAllAuthDescriptorsExclude()
        {
            // ToDo
        }

        public async void Sync()
        {
            // ToDo
        }

        private async Task SyncAssets()
        {
            this.Assets = await AssetBalance.GetByAccountId(this.Id, this.Session.Blockchain);
        }

        public AssetBalance GetAssetById(byte[] id)
        {
            return this.Assets.Find(assetBalance => assetBalance.Asset.GetId().Equals(id));
        }

        public async void TransferInputsToOutputs(List<GtvSerializable> inputs, List<GtvSerializable> outputs)
        {
            // ToDo
        }

        public async void Transfer(byte[] accountId, byte[] assetId, float amount)
        {
            // ToDo
        }

        public async void BurnTokens(byte[] assetId, float amount)
        {
            // ToDo
        }

        public async Task<dynamic> GetPaymentHistory()
        {
            
        }

        public async Task<PaymentHistoryIterator> GetPaymentHistoryIterator(int pageSize)
        {

        }

        public async Task XcTransfer(byte[] destinationChainId, byte[] destinationAccountId, byte[] assetId, float amount)
        {

        }

        /* Operation and query */
        public List<GtvSerializable> XcTransferOp(byte[] destinationChainId, byte[] destinationAccountId, byte[] assetId, float amount)
        {
            return null;
        }

        public List<GtvSerializable> AddAuthDescriptorOp(AuthDescriptor authDescriptor)
        {
            return null;
        }

        public static List<GtvSerializable> RegisterOp(AuthDescriptor authDescriptor)
        {
            return null;
        }
    }
}