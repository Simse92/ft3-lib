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
            
        }
    }

    public interface AuthDescriptor : GtvSerializable
    {
        // id: Buffer;
        // signers: PubKey[];
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

        public static async Task<List<Account>> GetByParticipantId(byte[] id, BlockchainSession session)
        {
            var accountIds = await session.Query(
                "ft3.get_accounts_by_participant_id",
                ("id", Util.ByteArrayToString(id))
            );

            List<Account> accounts = new List<Account>();

            foreach (var accountId in accountIds)
            {
                //TODO
            }
            return accounts;
        }

        public static async Task<List<Account>> GetByAuthDescriptor(byte[] id, BlockchainSession session)
        {
            var accountIds = await session.Query(
                "ft3.get_accounts_by_auth_descriptor_id",
                ("descriptor_id", Util.ByteArrayToString(id))
            );

            List<Account> accounts = new List<Account>();

            foreach (var accountId in accountIds)
            {
                //TODO
            }
            return accounts;
        }

        public static async Task<List<Account>> GetByIds(List<byte[]> ids, BlockchainSession session)
        {

        }

        public static async Account GetById(byte[] id, BlockchainSession session)
        {
            var account = await session.Query(
                "ft3.get_account_by_id",
                ("id", Util.ByteArrayToString(id))
            );

            if(account == null)
            {
                return null;
            }

            var authDescriptorFactory = new AuthDescriptorFactory();
        }

        public static async Task<Account> Register(AuthDescriptor authDescriptor, BlockchainSession session)
        {
            // await session.call(...this.registerOp(authDescriptor));
            // const account = new Account(authDescriptor.hash(), [authDescriptor], session);
            // await account.syncAssets();
            return null;
        }

    }

}