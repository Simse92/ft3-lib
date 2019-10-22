using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chromia.Postchain.Ft3
{
    public class ParamPaymentPair
    {
        public readonly PaymentParam param;
        public readonly PaymentOperation payment;

        public ParamPaymentPair(PaymentParam param, PaymentOperation payment)
        {
            this.param = param;
            this.payment = payment;
        }
    }

    public class PaymentHistorySyncManager
    {
        public readonly PaymentHistoryStore PaymentHistoryStore = new PaymentHistoryStoreLocalStorage();

        public async Task SyncAccount(byte[] id, Blockchain blockchain)
        {

        }

        private PaymentHistoryEntry[] MapShortEntriesToLongEntries(PaymentHistoryEntryShort[] entries)
        {
            return null;
        }

        private Dictionary<string, dynamic[]> GroupShortEntriesByTransactionRID(PaymentHistoryEntryShort[] entries)
        {
            return null;
        }

        private PaymentHistoryEntry[] PaymentHistoryEntriesFrom(PaymentHistoryEntryShort[] entries, byte[] chainId, byte[] accountId)
        {
            return null;
        }

        private ParamPaymentPair MatchPaymentHistoryEntryAndPaymentParam(PaymentHistoryEntryShort entry, ParamPaymentPair[] paramPair, byte[] accountId)
        {
            return null;
        }

        private PaymentHistoryEntry GetPaymentHistoryEntry(PaymentHistoryEntryShort entry, PaymentOperation payment)
        {
            return null;
        }

        private int GetHighestBlock(PaymentHistoryEntry[] entries, int lastBlock)
        {
            return -1;
        }

        private PaymentOperation[] GetPaymentsForChainAndAccountFromRawTransaction(string chainId, byte[] accountId, byte[] transactionData)
        {
            return null;
        }
    }
}