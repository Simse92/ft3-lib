namespace Chromia.Postchain.Ft3
{
    public class PaymentHistoryStoreLocalStorage : PaymentHistoryStore
    {
        //private entriesCache: {[key: string]: PaymentHistoryEntry[]} = {};


        public int GetCount(byte[] accountId)
        {
            return 0;
        }
        public PaymentHistoryIterator GetIterator(byte[] accountId, int pageSize)
        {
            return null;
        }

        public void Save(byte[] accountId, PaymentHistoryEntry[] paymentHistoryEntries)
        {

        }

        public PaymentHistoryEntry[] GetEntries(byte[] accountId, int start, int pageSize)
        {
            return null;
        }

        public dynamic GetSyncInfo(byte[] accountId)
        {
            return null;
        }

        public void SaveSyncInfo(byte[] accountId, dynamic syncInfo)
        {

        }

        private PaymentHistoryEntry[] GetEntriesFor(byte[] accountId)
        {
            return null;
        }

        private PaymentHistoryEntry[] LoadFromStore(byte[] id)
        {
            return null;
        }

        private void SaveToStore(byte[] id, PaymentHistoryEntry[] entries)
        {
            
        }

        private PaymentHistoryEntry MapToPaymentHistoryEntry(dynamic entry)
        {
            return null;
        }
    }
}
